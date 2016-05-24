using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace DevelopmentWithADot.AspNetSpeechRecognition
{
	[ConstructorNeedsTag(false)]
	public class SpeechRecognition : HtmlGenericControl, ICallbackEventHandler
	{
		public SpeechRecognition() : base("span")
		{
			this.OnClientSpeechRecognized = String.Empty;
			this.Mode = SpeechRecognitionMode.Desktop;
			this.Culture = String.Empty;
			this.SampleRate = 44100;
			this.Grammars = new String[0];
			this.Choices = new String[0];
		}

		public event EventHandler<SpeechRecognizedEventArgs> SpeechRecognized;

		[DefaultValue("")]
		public String Culture
		{
			get;
			set;
		}

		[DefaultValue(SpeechRecognitionMode.Desktop)]
		public SpeechRecognitionMode Mode
		{
			get;
			set;
		}

		[DefaultValue("")]
		public String OnClientSpeechRecognized
		{
			get;
			set;
		}

		[DefaultValue(44100)]
		public UInt32 SampleRate
		{
			get;
			set;
		}

		[TypeConverter(typeof(StringArrayConverter))]
		[DefaultValue("")]
		public String [] Grammars
		{
			get;
			private set;
		}

		[TypeConverter(typeof(StringArrayConverter))]
		[DefaultValue("")]
		public String[] Choices
		{
			get;
			set;
		}

		protected override void OnInit(EventArgs e)
		{
			if (this.Page.Items.Contains(typeof(SpeechRecognition)))
			{
				throw (new InvalidOperationException("There can be only one SpeechRecognition control on a page."));
			}

			var sm = ScriptManager.GetCurrent(this.Page);
			var reference = this.Page.ClientScript.GetCallbackEventReference(this, "sound", String.Format("function(result){{ {0}(JSON.parse(result)); }}", String.IsNullOrWhiteSpace(this.OnClientSpeechRecognized) ? "void" : this.OnClientSpeechRecognized), String.Empty, true);
			var script = String.Format("\nvar processor = document.getElementById('{0}'); processor.stopRecording = function(sampleRate) {{ window.stopRecording(processor, sampleRate ? sampleRate : 44100); }}; processor.startRecording = function() {{ window.startRecording(); }}; processor.process = function(sound){{ {1} }};\n", this.ClientID, reference);

			if (sm != null)
			{
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), String.Concat("process", this.ClientID), String.Format("Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function() {{ {0} }});\n", script), true);
			}
			else
			{
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), String.Concat("process", this.ClientID), script, true);
			}

			this.Page.ClientScript.RegisterClientScriptResource(this.GetType(), String.Concat(this.GetType().Namespace, ".Script.js"));
			this.Page.Items[typeof(SpeechRecognition)] = this;

			base.OnInit(e);
		}

		protected virtual void OnSpeechRecognized(SpeechRecognizedEventArgs e)
		{
			var handler = this.SpeechRecognized;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected SpeechRecognitionProvider GetProvider()
		{
			switch (this.Mode)
			{
				case SpeechRecognitionMode.Desktop:
					return (new DesktopSpeechRecognitionProvider(this));

				case SpeechRecognitionMode.Server:
					return (new ServerSpeechRecognitionProvider(this));
			}

			return (null);
		}

		#region ICallbackEventHandler Members

		String ICallbackEventHandler.GetCallbackResult()
		{
			AsyncOperationManager.SynchronizationContext = new SynchronizationContext();

			var filename = Path.GetTempFileName();
			var result = null as SpeechRecognitionResult;

			using (var engine = this.GetProvider())
			{
				var data = this.Context.Items["data"].ToString();

				using (var file = File.OpenWrite(filename))
				{
					var bytes = Convert.FromBase64String(data);
					file.Write(bytes, 0, bytes.Length);
				}

				result = engine.RecognizeFromWav(filename) ?? new SpeechRecognitionResult(String.Empty);
			}

			File.Delete(filename);

			var args = new SpeechRecognizedEventArgs(result);

			this.OnSpeechRecognized(args);

			var json = new JavaScriptSerializer().Serialize(result);

			return (json);
		}

		void ICallbackEventHandler.RaiseCallbackEvent(String eventArgument)
		{
			this.Context.Items["data"] = eventArgument;
		}

		#endregion
	}
}
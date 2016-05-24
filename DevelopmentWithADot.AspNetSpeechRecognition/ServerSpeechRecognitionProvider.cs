using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Recognition.SrgsGrammar;

namespace DevelopmentWithADot.AspNetSpeechRecognition
{
	public class ServerSpeechRecognitionProvider : SpeechRecognitionProvider
	{
		public ServerSpeechRecognitionProvider(SpeechRecognition recognition) : base(recognition)
		{
		}

		public override SpeechRecognitionResult RecognizeFromWav(String filename)
		{
			var engine = null as SpeechRecognitionEngine;

			if (String.IsNullOrWhiteSpace(this.Recognition.Culture) == true)
			{
				engine = new SpeechRecognitionEngine();
			}
			else
			{
				engine = new SpeechRecognitionEngine(CultureInfo.CreateSpecificCulture(this.Recognition.Culture));
			}

			using (engine)
			{
				foreach (var grammar in this.Recognition.Grammars)
				{
					var doc = new SrgsDocument(Path.Combine(HttpRuntime.AppDomainAppPath, grammar));
					engine.LoadGrammar(new Grammar(doc));
				}

				if (this.Recognition.Choices.Any() == true)
				{
					var choices = new Choices(this.Recognition.Choices.ToArray());
					engine.LoadGrammar(new Grammar(choices));
				}

				engine.SetInputToWaveFile(filename);

				var result = engine.Recognize();

				return ((result != null) ? new SpeechRecognitionResult(result.Text, result.Alternates.Select(x => x.Text).ToArray()) : null);
			}
		}
	}
}
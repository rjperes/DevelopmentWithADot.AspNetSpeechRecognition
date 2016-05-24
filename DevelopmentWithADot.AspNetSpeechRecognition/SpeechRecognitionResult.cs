using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentWithADot.AspNetSpeechRecognition
{
	public class SpeechRecognitionResult
	{
		public SpeechRecognitionResult(String text, params String [] alternates)
		{
			this.Text = text;
			this.Alternates = alternates.ToList();
		}

		public String Text
		{
			get;
			set;
		}

		public List<String> Alternates
		{
			get;
			private set;
		}
	}
}
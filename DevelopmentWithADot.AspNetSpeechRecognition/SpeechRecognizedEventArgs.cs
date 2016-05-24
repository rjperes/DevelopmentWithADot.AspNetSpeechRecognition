using System;

namespace DevelopmentWithADot.AspNetSpeechRecognition
{
	[Serializable]
	public sealed class SpeechRecognizedEventArgs : EventArgs
	{
		public SpeechRecognizedEventArgs(SpeechRecognitionResult result)
		{
			this.Result = result;
		}

		public SpeechRecognitionResult Result
		{
			get;
			private set;
		}
	}
}
using System;

namespace DevelopmentWithADot.AspNetSpeechRecognition
{
	public abstract class SpeechRecognitionProvider : IDisposable
	{
		protected SpeechRecognitionProvider(SpeechRecognition recognition)
		{
			this.Recognition = recognition;
		}

		~SpeechRecognitionProvider()
		{
			this.Dispose(false);
		}

		public abstract SpeechRecognitionResult RecognizeFromWav(String filename);

		protected SpeechRecognition Recognition
		{
			get;
			private set;
		}

		protected virtual void Dispose(Boolean disposing)
		{
		}

		void IDisposable.Dispose()
		{
			GC.SuppressFinalize(this);
			this.Dispose(true);
		}
	}
}
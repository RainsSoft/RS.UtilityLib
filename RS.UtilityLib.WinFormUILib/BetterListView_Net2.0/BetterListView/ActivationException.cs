using System;
using System.Runtime.Serialization;

namespace ComponentOwl.BetterListView
{

	internal class ActivationException : Exception
	{
		public ActivationException() {
		}

		public ActivationException(string message)
			: base(message) {
		}

		public ActivationException(string message, Exception innerException)
			: base(message, innerException) {
		}

		protected ActivationException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}
	}
}
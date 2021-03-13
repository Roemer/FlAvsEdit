using System;
using System.Runtime.Serialization;

namespace AviSynthCsharpWrapper
{
    public class AviSynthException : Exception
    {
        public AviSynthException()
        {
        }

        public AviSynthException(string message) : base(message)
        {
        }

        public AviSynthException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AviSynthException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

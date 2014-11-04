using System;
using System.Runtime.Serialization;

namespace RoslynDemos.Demo2
{
    [Serializable]
    internal class ExternalSystemException : Exception
    {
        public ExternalSystemException()
        {
        }

        public ExternalSystemException(string message) : base(message)
        {
        }

        public ExternalSystemException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ExternalSystemException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ErrorCode ErrorCode { get; internal set; }
    }
}
using System;
using System.Runtime.Serialization;

namespace NobreakTSSharaDDDWeb.HidRS232
{
    class NobreakNotConnectedException : Exception
    {
        public NobreakNotConnectedException()
        : base() { }

        public NobreakNotConnectedException(string message)
        : base(message) { }

        public NobreakNotConnectedException(string format, params object[] args)
        : base(string.Format(format, args)) { }

        public NobreakNotConnectedException(string message, Exception innerException)
        : base(message, innerException) { }

        public NobreakNotConnectedException(string format, Exception innerException, params object[] args)
        : base(string.Format(format, args), innerException) { }

        protected NobreakNotConnectedException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
    }
}

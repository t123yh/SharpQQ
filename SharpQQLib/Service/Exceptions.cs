using System;

namespace SharpQQ.Service
{
    public class QQException : Exception
    {
        public QQException()
        {
        }

        public QQException(string message) : base(message)
        {
        }

        public QQException(string message, Exception innerException)
        {
        }
    }

    public class LoginException : Exception
    {
        public int Code { get; }

        public LoginException(string message, int code) : base(message)
        {
            this.Code = code;
        }
    }
}
using System;

namespace Glass.Data.Exceptions
{
    public class LogoutException : Exception
    {
        public LogoutException(string message) : base(message)
        {
        }
    }
}

using System.Globalization;

namespace Core.Exceptions
{
    public class ISDException : Exception
    {
        public const string ErrorCode = "error_code";
        public ISDException()
        {

        }
        public ISDException(string message) : base(message)
        {
        }

        public ISDException(string message, params object[] args) : base(string.Format(CultureInfo.CurrentCulture,
            message, args))
        {
        }

        public ISDException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ISDException(string message, int code) : base(message)
        {
            Data.Add(ErrorCode, code);
        }
    }
}

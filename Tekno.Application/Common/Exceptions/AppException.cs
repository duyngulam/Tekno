namespace Tekno.Application.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }

        protected AppException(string message, int statusCode, string errorCode = "APP_ERROR")
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}

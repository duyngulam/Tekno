namespace Tekno.Api.Common.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }

        public object? Errors { get; }

        public AppException(string message, int statusCode = 400, object? errors = null) : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
        }
    }
}

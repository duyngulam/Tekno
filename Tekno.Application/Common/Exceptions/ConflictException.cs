namespace Tekno.Application.Common.Exceptions
{
    public class ConflictException : AppException
    {
        public ConflictException(string message, string errorCode = "CONFLICT_ERROR")
            : base(message, 409, errorCode)
        {
        }
    }
}

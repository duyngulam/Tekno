namespace Tekno.Application.Common.Exceptions
{
    public class ValidationException : AppException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException(IDictionary<string, string[]> errors)
            : base("Validation failed.", 400, "VALIDATION_ERROR")
        {
            Errors = errors;
        }
    }

}

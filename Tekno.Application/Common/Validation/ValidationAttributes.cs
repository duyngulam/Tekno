using System.ComponentModel.DataAnnotations;

namespace Tekno.Application.Common.Validation
{
    /// <summary>
    /// Validates that the coupon type is one of the allowed values
    /// </summary>
    public class CouponTypeValidationAttribute : ValidationAttribute
    {
        private static readonly string[] ValidTypes = { "FixedAmount", "Percentage", "FreeShipping" };

        public CouponTypeValidationAttribute()
        {
            ErrorMessage = "Invalid coupon type. Allowed values are: FixedAmount, Percentage, FreeShipping";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Coupon type is required");
            }

            var typeValue = value.ToString()!;
            
            if (!ValidTypes.Contains(typeValue, StringComparer.OrdinalIgnoreCase))
            {
                return new ValidationResult(
                    $"Invalid coupon type '{typeValue}'. Allowed values are: {string.Join(", ", ValidTypes)}");
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Validates password strength
    /// </summary>
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public int MinLength { get; set; } = 6;
        public bool RequireUppercase { get; set; } = false;
        public bool RequireNumber { get; set; } = false;
        public bool RequireSpecialChar { get; set; } = false;

        public StrongPasswordAttribute()
        {
            ErrorMessage = "Password does not meet strength requirements";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Password is required");
            }

            var password = value.ToString()!;
            var errors = new List<string>();

            if (password.Length < MinLength)
            {
                errors.Add($"Password must be at least {MinLength} characters long");
            }

            if (RequireUppercase && !password.Any(char.IsUpper))
            {
                errors.Add("Password must contain at least one uppercase letter");
            }

            if (RequireNumber && !password.Any(char.IsDigit))
            {
                errors.Add("Password must contain at least one number");
            }

            if (RequireSpecialChar && !password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                errors.Add("Password must contain at least one special character");
            }

            if (errors.Any())
            {
                return new ValidationResult(string.Join(". ", errors));
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Validates phone number format
    /// </summary>
    public class PhoneNumberValidationAttribute : ValidationAttribute
    {
        public PhoneNumberValidationAttribute()
        {
            ErrorMessage = "Invalid phone number format. Example: +84987654321 or 0987654321";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Optional field
            }

            var phoneNumber = value.ToString()!;
            
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return ValidationResult.Success;
            }

            // Basic validation: starts with + or digit, contains only digits, spaces, hyphens, parentheses
            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[\+]?[(]?[0-9]{1,4}[)]?[-\s\.]?[(]?[0-9]{1,4}[)]?[-\s\.]?[0-9]{1,9}$"))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

    /// <summary>
    /// Validates that end date is after start date
    /// </summary>
    public class DateRangeValidationAttribute : ValidationAttribute
    {
        public string StartDateProperty { get; set; } = "StartDate";

        public DateRangeValidationAttribute()
        {
            ErrorMessage = "End date must be after start date";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var endDate = (DateTime)value;
            var startDateProperty = validationContext.ObjectType.GetProperty(StartDateProperty);
            
            if (startDateProperty == null)
            {
                return ValidationResult.Success;
            }

            var startDateValue = startDateProperty.GetValue(validationContext.ObjectInstance);
            if (startDateValue == null)
            {
                return ValidationResult.Success;
            }

            var startDate = (DateTime)startDateValue;

            if (endDate <= startDate)
            {
                return new ValidationResult($"End date must be after start date ({startDate:yyyy-MM-dd})");
            }

            return ValidationResult.Success;
        }
    }
}

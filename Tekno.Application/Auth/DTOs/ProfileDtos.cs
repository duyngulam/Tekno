using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tekno.Application.Auth.DTOs
{
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<UserAddressDto> Addresses { get; set; } = new();
    }

    public class UserAddressDto
    {
        public int Id { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        
        // Location codes
        public int ProvinceCode { get; set; }
        public int DistrictCode { get; set; }
        public int WardCode { get; set; }
        
        // Cached names for display
        public string ProvinceName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string WardName { get; set; } = string.Empty;
        
        // Full formatted address
        public string FullAddress { get; set; } = string.Empty;
        
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateProfileDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
    }

    public class UpdateEmailDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string NewEmail { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string CurrentPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordDto
    {
        [Required]
        [MinLength(6, ErrorMessage = "Current password must be at least 6 characters")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// Update all profile information at once (fullname, phone, email, password)
    /// </summary>
    public class UpdateAllProfileDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? NewEmail { get; set; }

        [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;
    }

    public class CreateAddressDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address line must be between 5 and 200 characters")]
        public string AddressLine { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Province is required")]
        public int ProvinceCode { get; set; }

        [Required]
        public string ProvinceName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "District is required")]
        public int DistrictCode { get; set; }

        [Required]
        public string DistrictName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Ward is required")]
        public int WardCode { get; set; }

        [Required]
        public string WardName { get; set; } = string.Empty;

        public bool IsDefault { get; set; } = false;
    }

    public class UpdateAddressDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string RecipientName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Address line must be between 5 and 200 characters")]
        public string AddressLine { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Province is required")]
        public int ProvinceCode { get; set; }

        [Required]
        public string ProvinceName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "District is required")]
        public int DistrictCode { get; set; }

        [Required]
        public string DistrictName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Ward is required")]
        public int WardCode { get; set; }

        [Required]
        public string WardName { get; set; } = string.Empty;
    }

    // Remove ProvinceOptionDto - use LocationController instead
}

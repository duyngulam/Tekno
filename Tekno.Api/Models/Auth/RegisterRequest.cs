using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Api.Models.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Application.Auth.DTOs;
namespace Tekno.Api.Models.Auth
{
    public class AuthResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public static AuthResponse FromAppDto(UserDto dto)
        {
            return new AuthResponse
            {
                Id = dto.Id,
                Email = dto.Email,
                Role = dto.Role,
                Token = dto.Token,
                ExpiresAt = dto.ExpiresAt
            };
        }
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tekno.Application.Auth.Interfaces;
using Tekno.Domain.Auth;

namespace Tekno.Infrastructure.Auth
{
    public class JwtProvider : IJwtProvider
    {
        private readonly string _secret;
        private readonly int _expiryMinutes;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtProvider(IConfiguration config)
        {
            _secret = config["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey");
            _expiryMinutes = int.Parse(config["JwtSettings:ExpiryMinutes"] ?? "60");
            _issuer = config["JwtSettings:Issuer"] ?? "TeknoApi";
            _audience = config["JwtSettings:Audience"] ?? "TeknoUsers";
        }

        public (string Token, DateTime ExpiresAt) GenerateToken(User user)
        {
            // Build claims safely - Role navigation may be null if not loaded
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Email ?? string.Empty)
            };

            var roleName = user.Role?.Name;
            if (string.IsNullOrWhiteSpace(roleName))
            {
                // fallback to a generic role label or use role id as hint
                roleName = user.RoleId > 0 ? $"role_{user.RoleId}" : "User";
            }

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }
    }
}

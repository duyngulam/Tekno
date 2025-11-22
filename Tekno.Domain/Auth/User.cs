using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Tekno.Domain.Auth
{
    public class User
    {
        public int Id { get; private set; }
        public string Fullname { get; private set; } = string.Empty;
        public string Email { get; private set; }
        public string? PhoneNumber { get; private set; }
        public string PasswordHash { get; private set; }
        public int RoleId { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        public Role Role { get; private set; } = null!;   // navigation property
        public ICollection<UserAddress> Addresses { get; private set; } = new List<UserAddress>();

        private User() { } // EF Core

        public User(string email, string passwordHash, int roleId = 2)
        {
            Email = email;
            PasswordHash = passwordHash;
            RoleId = roleId; // default to Customer
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateProfile(string fullname, string? phoneNumber)
        {
            Fullname = fullname?.Trim() ?? string.Empty;
            PhoneNumber = phoneNumber?.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            Email = email.Trim().ToLowerInvariant();
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePassword(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

            PasswordHash = passwordHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddAddress(UserAddress address)
        {
            Addresses.Add(address);
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDefaultAddress(int addressId)
        {
            foreach (var addr in Addresses)
            {
                addr.SetDefault(addr.Id == addressId);
            }
            UpdatedAt = DateTime.UtcNow;
        }
    }
}

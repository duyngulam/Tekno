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
        public string PasswordHash { get; private set; }
        public int RoleId { get; private set; }

        public Role Role { get; private set; }   // navigation property

        private User() { } // EF Core

        public User(string email, string passwordHash, int roleId = 2)
        {
            Email = email;
            PasswordHash = passwordHash;
            RoleId = roleId; // default to Customer
        }
    }
}

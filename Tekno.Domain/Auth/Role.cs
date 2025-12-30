using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Auth
{
    public class Role
    {
        public const int AdminId = 1;
        public const int CustomerId = 2;

        public const string AdminName = "Admin";
        public const string CustomerName = "Customer";

        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        private Role() { }
        public Role(string name) => Name = name;

        public static Role CreateAdmin() => new Role(AdminName);
        public static Role CreateCustomer() => new Role(CustomerName);
    }
}

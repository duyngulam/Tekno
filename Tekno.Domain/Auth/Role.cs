using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekno.Domain.Auth
{
    public class Role
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        private Role() { }
        public Role(string name) => Name = name;
    }
}

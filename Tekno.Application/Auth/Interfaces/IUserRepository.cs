using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekno.Domain.Auth;

namespace Tekno.Application.Auth.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task AddAsync(User user);
        Task<bool> ExistsAsync(string username);
    }
}

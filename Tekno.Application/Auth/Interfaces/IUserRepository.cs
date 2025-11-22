using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Domain.Auth;

namespace Tekno.Application.Auth.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdWithAddressesAsync(int id);
        Task<Role?> GetRoleByNameAsync(string roleName);
        Task AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> ExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email, int excludeUserId);
        
        // Address operations
        Task<UserAddress?> GetAddressByIdAsync(int addressId);
        Task<UserAddress> AddAddressAsync(UserAddress address);
        Task<UserAddress> UpdateAddressAsync(UserAddress address);
        Task<bool> DeleteAddressAsync(int addressId);
    }
}

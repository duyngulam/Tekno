using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Auth.Interfaces;
using Tekno.Domain.Auth;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Auth
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email.ToLower());
        }

        public async Task<User?> GetByIdWithAddressesAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Addresses)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Email == username.ToLower());
        }

        public async Task<bool> EmailExistsAsync(string email, int excludeUserId)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email.ToLower() && u.Id != excludeUserId);
        }

        // Address operations
        public async Task<UserAddress?> GetAddressByIdAsync(int addressId)
        {
            return await _context.Set<UserAddress>()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == addressId);
        }

        public async Task<UserAddress> AddAddressAsync(UserAddress address)
        {
            _context.Set<UserAddress>().Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<UserAddress> UpdateAddressAsync(UserAddress address)
        {
            _context.Set<UserAddress>().Update(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<bool> DeleteAddressAsync(int addressId)
        {
            var address = await _context.Set<UserAddress>().FindAsync(addressId);
            if (address == null) return false;

            _context.Set<UserAddress>().Remove(address);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

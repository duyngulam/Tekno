using Microsoft.EntityFrameworkCore;
using Tekno.Application.Auth.Interfaces;
using Tekno.Domain.Auth;

namespace Tekno.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.Include(u => u.Role)
                                       .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}

using Microsoft.EntityFrameworkCore;
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

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role)  
                .FirstOrDefaultAsync(u => u.Username == username);
        }


        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tekno.Application.Cart.Interface;
using Tekno.Domain.Cart;
using Tekno.Infrastructure.Persistence;

namespace Tekno.Infrastructure.Cart
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserCart?> GetByUserIdAsync(int userId)
        {
            return await _context.Set<UserCart>()
                .Include(c => c.Items)
                    .ThenInclude(i => i.Variant)
                        .ThenInclude(v => v.Product)
                            .ThenInclude(p => p.Images)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Variant)
                        .ThenInclude(v => v.Product)
                            .ThenInclude(p => p.Brand)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Variant)
                        .ThenInclude(v => v.Product)
                            .ThenInclude(p => p.Category)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Variant)
                        .ThenInclude(v => v.VariantAttributes)
                            .ThenInclude(va => va.Attribute)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Variant)
                        .ThenInclude(v => v.VariantAttributes)
                            .ThenInclude(va => va.Value)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<UserCart> CreateAsync(UserCart cart)
        {
            _context.Set<UserCart>().Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<UserCart> UpdateAsync(UserCart cart)
        {
            _context.Set<UserCart>().Update(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> DeleteAsync(int cartId)
        {
            var cart = await _context.Set<UserCart>().FindAsync(cartId);
            if (cart == null) return false;

            _context.Set<UserCart>().Remove(cart);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CartItem?> GetCartItemAsync(int cartId, int variantId)
        {
            return await _context.Set<CartItem>()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.CartId == cartId && i.VariantId == variantId);
        }

        public async Task<bool> RemoveItemAsync(int cartId, int variantId)
        {
            var item = await _context.Set<CartItem>()
                .FirstOrDefaultAsync(i => i.CartId == cartId && i.VariantId == variantId);
            
            if (item == null) return false;

            _context.Set<CartItem>().Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Wishlist>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<Wishlist>()
                .Include(w => w.Variant)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.Images)
                .Include(w => w.Variant)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.Brand)
                .Include(w => w.Variant)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.Category)
                .Include(w => w.Variant)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Attribute)
                .Include(w => w.Variant)
                    .ThenInclude(v => v.VariantAttributes)
                        .ThenInclude(va => va.Value)
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();
        }

        public async Task<Wishlist?> GetByUserAndVariantAsync(int userId, int variantId)
        {
            return await _context.Set<Wishlist>()
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.VariantId == variantId);
        }

        public async Task<Wishlist> AddAsync(Wishlist wishlist)
        {
            _context.Set<Wishlist>().Add(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        public async Task<bool> RemoveAsync(int userId, int variantId)
        {
            var wishlist = await _context.Set<Wishlist>()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.VariantId == variantId);
            
            if (wishlist == null) return false;

            _context.Set<Wishlist>().Remove(wishlist);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsInWishlistAsync(int userId, int variantId)
        {
            return await _context.Set<Wishlist>()
                .AsNoTracking()
                .AnyAsync(w => w.UserId == userId && w.VariantId == variantId);
        }
    }
}

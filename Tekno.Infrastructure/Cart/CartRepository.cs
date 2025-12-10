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
            // Load the existing cart entity (will be tracked)
            var existing = await _context.Set<UserCart>()
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cart.Id);

            if (existing == null)
            {
                throw new InvalidOperationException($"Cart with ID {cart.Id} not found");
            }

            // Sync cart items (update quantities, add new, remove deleted)
            // Remove items that are no longer in the cart
            var itemsToRemove = existing.Items
                .Where(ei => !cart.Items.Any(ci => ci.VariantId == ei.VariantId))
                .ToList();
            
            foreach (var item in itemsToRemove)
            {
                existing.Items.Remove(item);
            }

            // Update existing items or add new ones
            foreach (var cartItem in cart.Items)
            {
                var existingItem = existing.Items
                    .FirstOrDefault(i => i.VariantId == cartItem.VariantId);

                if (existingItem != null)
                {
                    // Update existing item's quantity and price
                    existingItem.UpdateQuantity(cartItem.Quantity);
                    existingItem.UpdatePrice(cartItem.Price);
                }
                else
                {
                    // Add new item
                    var newItem = new CartItem(existing.Id, cartItem.VariantId, cartItem.Quantity, cartItem.Price);
                    existing.Items.Add(newItem);
                }
            }

            // Mark cart as updated
            existing.MarkAsUpdated();

            await _context.SaveChangesAsync();
            return existing;
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
                .Include(w => w.Product)
                    .ThenInclude(p => p.Images)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Brand)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Category)
                .Include(w => w.Product)
                    .ThenInclude(p => p.Variants)
                .AsNoTracking()
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();
        }

        public async Task<Wishlist?> GetByUserAndProductAsync(int userId, int productId)
        {
            return await _context.Set<Wishlist>()
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<Wishlist> AddAsync(Wishlist wishlist)
        {
            _context.Set<Wishlist>().Add(wishlist);
            await _context.SaveChangesAsync();
            return wishlist;
        }

        public async Task<bool> RemoveAsync(int userId, int productId)
        {
            var wishlist = await _context.Set<Wishlist>()
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
            
            if (wishlist == null) return false;

            _context.Set<Wishlist>().Remove(wishlist);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsInWishlistAsync(int userId, int productId)
        {
            return await _context.Set<Wishlist>()
                .AsNoTracking()
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }
    }
}

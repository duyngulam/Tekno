using System.Collections.Generic;
using System.Threading.Tasks;
using Tekno.Domain.Cart;

namespace Tekno.Application.Cart.Interface
{
    public interface ICartRepository
    {
        Task<UserCart?> GetByUserIdAsync(int userId);
        Task<UserCart> CreateAsync(UserCart cart);
        Task<UserCart> UpdateAsync(UserCart cart);
        Task<bool> DeleteAsync(int cartId);
        Task<CartItem?> GetCartItemAsync(int cartId, int variantId);
        Task<bool> RemoveItemAsync(int cartId, int variantId);
    }

    public interface IWishlistRepository
    {
        Task<List<Wishlist>> GetByUserIdAsync(int userId);
        Task<Wishlist?> GetByUserAndProductAsync(int userId, int productId);
        Task<Wishlist> AddAsync(Wishlist wishlist);
        Task<bool> RemoveAsync(int userId, int productId);
        Task<bool> IsInWishlistAsync(int userId, int productId);
    }
}

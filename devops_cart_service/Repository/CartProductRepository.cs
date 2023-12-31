using devops_cart_service.Data;
using devops_cart_service.Models;
using Microsoft.EntityFrameworkCore;

namespace devops_cart_service.Repository
{
    public class CartProductRepository : ICartProductRepository
    {
        private readonly ApplicationDbContext _db;
        public CartProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ICollection<CartProduct>> GetCartProductsByCartIdAsync(int cartId)
        {
            return await _db.CartProducts.Where(u => u.CartId == cartId && !u.IsDeleted).ToListAsync();
        }

        public async Task CreateCartProductAsync(CartProduct cartProduct)
        {
            cartProduct.CreatedAt = DateTimeOffset.Now;
            cartProduct.UpdatedAt = DateTimeOffset.Now;
            await _db.CartProducts.AddAsync(cartProduct);
            await SaveAsync();
        }

        public async Task UpdateCartProductAsync(CartProduct cartProduct)
        {
            cartProduct.UpdatedAt = DateTimeOffset.Now;
            _db.CartProducts.Update(cartProduct);
            await SaveAsync();
        }


        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
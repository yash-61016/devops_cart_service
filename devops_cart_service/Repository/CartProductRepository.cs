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

        public async Task<CartProduct> GetCartProductByIdAsync(int id)
        {
            return await _db.CartProducts.FirstOrDefaultAsync(u => u.CartProductId == id);
        }

        public async Task<ICollection<CartProduct>> GetCartProductsByCartIdAsync(int cartId)
        {
            return await _db.CartProducts.Where(u => u.CartId == cartId).ToListAsync();
        }

        public async Task CreateCartProductAsync(CartProduct cartProduct)
        {
            await _db.CartProducts.AddAsync(cartProduct);
            await SaveAsync();
        }

        public async Task UpdateCartProductAsync(CartProduct cartProduct)
        {
            _db.CartProducts.Update(cartProduct);
            await SaveAsync();
        }

        public async Task DeleteCartProductAsync(CartProduct cartProduct)
        {
            _db.CartProducts.Remove(cartProduct);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
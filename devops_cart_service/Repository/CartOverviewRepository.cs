using devops_cart_service.Data;
using devops_cart_service.Models;
using devops_cart_service.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace devops_cart_service.Repository
{
    public class CartOverviewRepository : ICartOverviewRepository
    {
        private readonly ApplicationDbContext _db;
        public CartOverviewRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CartOverview> GetCartOverviewByIdAsync(int id)
        {
            return await _db.CartOverviews.FirstOrDefaultAsync(u => u.CartId == id);
        }

        public async Task<CartOverview> GetCartOverviewByUserIdAsync(int userId)
        {
            return await _db.CartOverviews.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task CreateCartOverviewAsync(CartOverview cartOverview)
        {
            await _db.CartOverviews.AddAsync(cartOverview);
            await SaveAsync();
        }

        public async Task UpdateCartOverviewAsync(CartOverview cartOverview)
        {
            _db.CartOverviews.Update(cartOverview);
            await SaveAsync();
        }

        public async Task DeleteCartOverviewAsync(CartOverview cartOverview)
        {
            _db.CartOverviews.Remove(cartOverview);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
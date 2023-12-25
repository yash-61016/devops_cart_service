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
            return await _db.CartOverviews.FirstOrDefaultAsync(u => u.CartId == id && !u.IsDeleted) ?? throw new Exception("CartOverview not found");
        }

        public async Task<CartOverview> GetCartOverviewByUserIdAsync(int userId)
        {
            return await _db.CartOverviews.FirstOrDefaultAsync(u => u.UserId == userId && !u.IsCheckedOut && !u.IsDeleted) ?? throw new Exception("CartOverview not found");
        }

        public async Task CreateCartOverviewAsync(CartOverview cartOverview)
        {
            cartOverview.CreatedAt = DateTimeOffset.Now;
            cartOverview.UpdatedAt = DateTimeOffset.Now;
            await _db.CartOverviews.AddAsync(cartOverview);
            await SaveAsync();
        }

        public async Task UpdateCartOverviewAsync(CartOverview cartOverview)
        {
            cartOverview.UpdatedAt = DateTimeOffset.Now;
            _db.CartOverviews.Update(cartOverview);
            await SaveAsync();
        }


        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
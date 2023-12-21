using devops_cart_service.Models;

namespace devops_cart_service.Repository.IRepository
{
    public interface ICartOverviewRepository
    {
        Task<CartOverview> GetCartOverviewByIdAsync(int id);
        Task<CartOverview> GetCartOverviewByUserIdAsync(int userId);
        Task CreateCartOverviewAsync(CartOverview cartOverview);
        Task UpdateCartOverviewAsync(CartOverview cartOverview);
        Task DeleteCartOverviewAsync(CartOverview cartOverview);
        Task SaveAsync();
    }
}
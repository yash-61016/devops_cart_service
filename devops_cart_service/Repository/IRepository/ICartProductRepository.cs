using devops_cart_service.Models;
using devops_cart_service.Repository.IRepository;

namespace devops_cart_service.Repository
{
    public interface ICartProductRepository
    {
        Task<ICollection<CartProduct>> GetCartProductsByCartIdAsync(int cartId);
        Task CreateCartProductAsync(CartProduct cartProduct);
        Task UpdateCartProductAsync(CartProduct cartProduct);
        Task SaveAsync();
    }
}
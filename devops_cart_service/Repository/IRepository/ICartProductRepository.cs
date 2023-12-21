using devops_cart_service.Models;
using devops_cart_service.Repository.IRepository;

namespace devops_cart_service.Repository
{
    public interface ICartProductRepository
    {
        Task<CartProduct> GetCartProductByIdAsync(int id);
        Task<ICollection<CartProduct>> GetCartProductsByCartIdAsync(int cartId);
        Task CreateCartProductAsync(CartProduct cartProduct);
        Task UpdateCartProductAsync(CartProduct cartProduct);
        Task DeleteCartProductAsync(CartProduct cartProduct);
        Task SaveAsync();
    }
}
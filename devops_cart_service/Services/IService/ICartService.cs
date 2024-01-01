using devops_cart_service.Models;
using devops_cart_service.Models.Dto;

namespace devops_cart_service.Services.IService
{
    public interface ICartService
    {
        Task<APIResponse> CreateCartAsync(CartCreateDto cart_C_DTO);
        Task<APIResponse> GetCartAsync(int userId);
        Task<APIResponse> UpdateCartAsync(CartUpdateDto cart_U_DTO);
        Task<APIResponse> DeleteCartAsync(int cartId);
    }
}
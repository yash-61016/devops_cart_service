using System.Net;
using AutoMapper;
using devops_cart_service.Models;
using devops_cart_service.Models.Dto;
using devops_cart_service.Repository;
using devops_cart_service.Repository.IRepository;
using devops_cart_service.Services.IService;

namespace devops_cart_service.Services
{
    public class CartService : ICartService
    {
        private readonly ICartOverviewRepository _cartOverviewRepo;
        private readonly ICartProductRepository _cartProductRepo;
        private readonly IMapper _mapper;

        public CartService(ICartOverviewRepository cartOverviewRepo, ICartProductRepository cartProductRepo, IMapper mapper)
        {
            _cartOverviewRepo = cartOverviewRepo;
            _cartProductRepo = cartProductRepo;
            _mapper = mapper;
        }

        public async Task<APIResponse> CreateCartAsync(CartCreateDto cart_C_DTO)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var cartOverview = _mapper.Map<CartOverview>(cart_C_DTO.CartOverview);
                var cartProducts = _mapper.Map<IEnumerable<CartProduct>>(cart_C_DTO.CartProducts);

                try
                {
                    var existingCartOverview = await _cartOverviewRepo.GetCartOverviewByUserIdAsync(cartOverview.UserId);
                    if (existingCartOverview != null && !existingCartOverview.IsDeleted)
                    {
                        existingCartOverview.IsDeleted = true;
                        await _cartOverviewRepo.UpdateCartOverviewAsync(existingCartOverview);
                    }
                }
                catch (Exception ex)
                {
                    // TODO log exception
                }
                await _cartOverviewRepo.CreateCartOverviewAsync(cartOverview);
                foreach (var cartProduct in cartProducts)
                {
                    cartProduct.CartId = cartOverview.CartId;
                    await _cartProductRepo.CreateCartProductAsync(cartProduct);
                }
                var cart = new Cart
                {
                    CartOverview = cartOverview,
                    CartProducts = cartProducts
                };

                response.Result = _mapper.Map<CartDto>(cart);
                // response.Result = existingCartOverview;
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
                return response;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }

        public async Task<APIResponse> DeleteCartAsync(int cartId)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var cartOverview = await _cartOverviewRepo.GetCartOverviewByIdAsync(cartId);
                cartOverview.IsDeleted = true;
                await _cartOverviewRepo.UpdateCartOverviewAsync(cartOverview);

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.NoContent;
                return response;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }

        public async Task<APIResponse> GetCartAsync(int userId)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var cartOverview = await _cartOverviewRepo.GetCartOverviewByUserIdAsync(userId);
                var _cartOverviewId = cartOverview.CartId;
                var cartProducts = await _cartProductRepo.GetCartProductsByCartIdAsync(_cartOverviewId);
                var cart = new Cart
                {
                    CartOverview = cartOverview,
                    CartProducts = cartProducts
                };
                response.Result = _mapper.Map<CartDto>(cart);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }

        public async Task<APIResponse> UpdateCartAsync(CartUpdateDto cart_U_DTO)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var cartOverview = _mapper.Map<CartOverview>(cart_U_DTO.CartOverview);
                var cartProducts = _mapper.Map<IEnumerable<CartProduct>>(cart_U_DTO.CartProducts);
                await _cartOverviewRepo.UpdateCartOverviewAsync(cartOverview);
                foreach (var cartProduct in cartProducts)
                {
                    cartProduct.CartId = cartOverview.CartId;
                    await _cartProductRepo.UpdateCartProductAsync(cartProduct);
                }

                if (cartOverview.IsCheckedOut)
                {
                    await _cartOverviewRepo.CheckoutCartOverviewAsync(cartOverview);
                }
                var cart = new Cart
                {
                    CartOverview = cartOverview,
                    CartProducts = cartProducts
                };

                response.Result = _mapper.Map<CartDto>(cart);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                return response;
            }
        }
    }
}
using System.Net;
using AutoMapper;
using devops_cart_service.Filters;
using devops_cart_service.Models;
using devops_cart_service.Models.Dto;
using devops_cart_service.Repository;
using devops_cart_service.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace devops_cart_service.Endpoints
{
    public static class CartEndpoints
    {
        public static void ConfigureCartEndpoints(this WebApplication app)
        {
            app.MapPost("/api/cart/create", CreateCart)
            .WithName("CreateCart")
            .Accepts<CartCreateDto>("application/json")
            .Produces<APIResponse>(201)
            .Produces(400);

            app.MapGet("/api/cart/user-{userId}", GetCart)
            .WithName("GetCart")
            .Produces<APIResponse>(200)
            .Produces(400);

            app.MapPut("/api/cart/update", UpdateCart)
            .WithName("UpdateCart")
            .Accepts<CartDto>("application/json")
            .Produces<APIResponse>(200)
            .Produces(400);

            app.MapDelete("/api/cart/{cartId}", DeleteCart)
            .WithName("DeleteCart")
            .Produces<APIResponse>(204)
            .Produces(400);

        }

        private async static Task<IResult> CreateCart(
            ICartOverviewRepository _cartOverviewRepo,
            ICartProductRepository _cartProductRepo,
            IMapper _mapper,
                 [FromBody] CartCreateDto cart_C_DTO)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var cartOverview = _mapper.Map<CartOverview>(cart_C_DTO.CartOverview);
                var cartProducts = _mapper.Map<IEnumerable<CartProduct>>(cart_C_DTO.CartProducts);
                cartOverview.CreatedAt = DateTimeOffset.Now;
                await _cartOverviewRepo.CreateCartOverviewAsync(cartOverview);
                foreach (var cartProduct in cartProducts)
                {
                    cartProduct.CartId = cartOverview.CartId;
                    cartProduct.CreatedAt = DateTimeOffset.Now;
                    await _cartProductRepo.CreateCartProductAsync(cartProduct);
                }
                var cart = new Cart
                {
                    CartOverview = cartOverview,
                    CartProducts = cartProducts
                };
                response.Result = _mapper.Map<CartDto>(cart);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                return Results.BadRequest(response);
            }
            return Results.Ok(response);
        }

        private async static Task<IResult> GetCart(
            ICartOverviewRepository _cartOverviewRepo,
            ICartProductRepository _cartProductRepo,
            IMapper _mapper,
            [FromRoute] int userId)
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
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                return Results.BadRequest(response);
            }
            return Results.Ok(response);
        }

        private async static Task<IResult> UpdateCart(
            ICartOverviewRepository _cartOverviewRepo,
            ICartProductRepository _cartProductRepo,
            IMapper _mapper,
            [FromBody] CartUpdateDto cart_DTO)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var cartOverview = _mapper.Map<CartOverview>(cart_DTO.CartOverview);
                var cartProducts = _mapper.Map<IEnumerable<CartProduct>>(cart_DTO.CartProducts);
                cartOverview.UpdatedAt = DateTimeOffset.Now;
                await _cartOverviewRepo.UpdateCartOverviewAsync(cartOverview);
                foreach (var cartProduct in cartProducts)
                {
                    cartProduct.CartId = cartOverview.CartId;
                    cartProduct.UpdatedAt = DateTimeOffset.Now;
                    await _cartProductRepo.UpdateCartProductAsync(cartProduct);
                }
                var cart = new Cart
                {
                    CartOverview = cartOverview,
                    CartProducts = cartProducts
                };

                response.Result = _mapper.Map<CartDto>(cart);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                return Results.BadRequest(response);
            }
            return Results.Ok(response);
        }

        private async static Task<IResult> DeleteCart(
            ICartOverviewRepository _cartOverviewRepo,
            IMapper _mapper,
            [FromRoute] int cartId)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            try
            {
                var cartOverview = await _cartOverviewRepo.GetCartOverviewByIdAsync(cartId);
                cartOverview.IsDeleted = true;
                cartOverview.UpdatedAt = DateTimeOffset.Now;
                await _cartOverviewRepo.UpdateCartOverviewAsync(cartOverview);

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.NoContent;
            }
            catch (Exception ex)
            {
                response.ErrorMessages.Add(ex.Message);
                return Results.BadRequest(response);
            }
            return Results.Ok(response);
        }
    }
}
using System.Net;
using AutoMapper;
using devops_cart_service.Filters;
using devops_cart_service.Models;
using devops_cart_service.Models.Dto;
using devops_cart_service.Repository;
using devops_cart_service.Repository.IRepository;
using devops_cart_service.Services.IService;
using Microsoft.AspNetCore.Mvc;

namespace devops_cart_service.Endpoints
{
    public static class CartEndpoints
    {
        public static void ConfigureCartEndpoints(this WebApplication app)
        {
            app.MapPost("/cart/create", CreateCart)
            .WithName("CreateCart")
            .Accepts<CartCreateDto>("application/json")
            .Produces<APIResponse>(201)
            .Produces(400)
            .RequireAuthorization();

            app.MapGet("/cart/user-{userId}", GetCart)
            .WithName("GetCart")
            .Produces<APIResponse>(200)
            .Produces(400)
            .RequireAuthorization();

            app.MapPut("/cart/update", UpdateCart)
            .WithName("UpdateCart")
            .Accepts<CartDto>("application/json")
            .Produces<APIResponse>(200)
            .Produces(400)
            .RequireAuthorization();

            app.MapDelete("/cart/{cartId}", DeleteCart)
            .WithName("DeleteCart")
            .Produces<APIResponse>(204)
            .Produces(400)
            .RequireAuthorization();

        }

        private async static Task<IResult> CreateCart(
            ICartService _cartService,
                 [FromBody] CartCreateDto cart_C_DTO)
        {
            var result = await _cartService.CreateCartAsync(cart_C_DTO);
            return result.IsSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
        }

        private async static Task<IResult> GetCart(
            ICartService _cartService,
            [FromRoute] int userId)
        {
            var result = await _cartService.GetCartAsync(userId);
            return result.IsSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
        }

        private async static Task<IResult> UpdateCart(
            ICartService _cartService,
            [FromBody] CartUpdateDto cart_U_DTO)
        {
            var result = await _cartService.UpdateCartAsync(cart_U_DTO);
            return result.IsSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
        }

        private async static Task<IResult> DeleteCart(
            ICartService _cartService,
            [FromRoute] int cartId)
        {
            var result = await _cartService.DeleteCartAsync(cartId);
            return result.IsSuccess ? TypedResults.NoContent() : TypedResults.BadRequest(result);
        }

    }
}
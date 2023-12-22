using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using devops_cart_service.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace devops_cart_service.Filters
{
    public class BasicValidator<T> : IEndpointFilter where T : class
    {
        private readonly IValidator<T> _validator;

        public BasicValidator(IValidator<T> validator)
        {
            _validator = validator;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
            var contextObj = context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T));

            if (contextObj == null)
            {
                return Results.BadRequest(response);
            }
            var result = await _validator.ValidateAsync((T)contextObj);
            if (!result.IsValid)
            {
                response.ErrorMessages.Add(result.Errors.FirstOrDefault()?.ToString() ?? string.Empty);
                return Results.BadRequest(response);
            }
            return await next(context);
        }
    }

}
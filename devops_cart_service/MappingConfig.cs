using AutoMapper;
using devops_cart_service.Models;
using devops_cart_service.Models.Dto;

namespace devops_cart_service
{
    public class MappingConfig : Profile
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartOverview, CartOverviewDto>().ReverseMap();
                config.CreateMap<CartOverview, CartOverviewCreateDto>().ReverseMap();
                config.CreateMap<CartOverview, CartOverviewUpdateDto>().ReverseMap();
                config.CreateMap<CartProduct, CartProductDto>().ReverseMap();
                config.CreateMap<CartProduct, CartProductCreateDto>().ReverseMap();
                config.CreateMap<CartProduct, CartProductUpdateDto>().ReverseMap();
                config.CreateMap<Cart, CartDto>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}

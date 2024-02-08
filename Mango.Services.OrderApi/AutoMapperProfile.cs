using AutoMapper;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Models.DTO;

namespace Mango.Services.OrderApi
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<OrderHeaderDTO, CartHeaderDTO>().ForMember(dest => dest.CartTotal, u=> u.MapFrom(src => src.OrderTotal)).ReverseMap();
            CreateMap<CartDetailsDTO, OrderDetailsDTO>().ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name));
            CreateMap<OrderDetailsDTO, CartDetailsDTO>();
            CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
            CreateMap<OrderDetails, OrderDetailsDTO>().ReverseMap();
        }
    }
}

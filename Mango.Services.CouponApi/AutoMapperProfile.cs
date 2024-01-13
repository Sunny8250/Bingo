using AutoMapper;
using Mango.Services.CouponApi.Models;
using Mango.Services.CouponApi.Models.DTO;

namespace Mango.Services.CouponApi
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Coupon, CouponDTO>().ReverseMap();
            CreateMap<Coupon, AddCouponDTO>().ReverseMap();
            CreateMap<Coupon, UpdateCouponDTO>().ReverseMap();
        }
    }
}

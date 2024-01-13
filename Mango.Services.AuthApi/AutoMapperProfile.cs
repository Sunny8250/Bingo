using AutoMapper;
using Mango.Services.AuthApi.Models;
using Mango.Services.AuthApi.Models.DTO;
using Mango.Services.CouponApi.Models;
using Mango.Services.CouponApi.Models.DTO;

namespace Mango.Services.AuthApi
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
        }
    }
}

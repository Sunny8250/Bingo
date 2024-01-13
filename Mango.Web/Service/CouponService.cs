using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> CreateCouponsAsync(CouponDTO couponDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.CouponApiBaseUrl + "/api/coupon/",
                Data = couponDto
            });
        }

        public async Task<ResponseDTO?> DeleteCouponsAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.CouponApiBaseUrl + "/api/coupon/" + id,
            });
        }

        public async Task<ResponseDTO?> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponApiBaseUrl + "/api/coupon",
            });
        }

        public async Task<ResponseDTO?> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponApiBaseUrl + "/api/coupon/"+couponCode,
            });
        }

        public async Task<ResponseDTO?> GetCouponByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.CouponApiBaseUrl + "/api/coupon/" + id,
            });
        }

        public async Task<ResponseDTO?> UpdateCouponsAsync(CouponDTO couponDto)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.PUT,
                Url = SD.CouponApiBaseUrl + "/api/coupon/",
                Data = couponDto
            });
        }
    }
}

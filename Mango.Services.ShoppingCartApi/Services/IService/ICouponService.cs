using Mango.Services.ShoppingCartApi.Models.DTO;

namespace Mango.Services.ShoppingCartApi.Services.IService
{
    public interface ICouponService
    {
        Task<CouponDTO> GetCouponByCodeAsync(string couponCode);
    }
}

using Mango.Services.ShoppingCartApi.Models.DTO;
using Mango.Services.ShoppingCartApi.Services.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartApi.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDTO> GetCouponByCodeAsync(string couponCode)
        {
            HttpClient client = _httpClientFactory.CreateClient("Coupon");
            HttpResponseMessage? response = await client.GetAsync($"/api/coupon/{couponCode}");

            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if (resp.IsSuccess)
            {
                var coupon = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(resp.Result));
                return coupon;
            }
            return null;
        }
    }
}

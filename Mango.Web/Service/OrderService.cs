using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> CreateOrderAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.OrderApiBaseUrl + "/api/order/CreateOrder",
                Data = cartDTO
            });
        }

        public async Task<ResponseDTO?> CreateStripeSessionAsync(StripeRequestDTO stripeRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.OrderApiBaseUrl + "/api/order/CreateStripeSession",
                Data = stripeRequestDTO
            });
        }

        public async Task<ResponseDTO?> ValidateStripeSessionAsync(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.OrderApiBaseUrl + "/api/order/ValidateStripeSession",
                Data = orderHeaderId
            });
        }
    }
}

using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartApiBaseUrl + "/api/cart/ApplyCoupon",
                Data = cartDTO
            });
        }
        public async Task<ResponseDTO?> EmailCartAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartApiBaseUrl + "/api/cart/EmailCartRequest",
                Data = cartDTO
            });
        }

        public async Task<ResponseDTO?> CartUpsertAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartApiBaseUrl + "/api/cart/CartUpsert",
                Data = cartDTO
            });
        }

        public async Task<ResponseDTO?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ShoppingCartApiBaseUrl + "/api/cart/GetCartById/"+userId
            });
        }

        public async Task<ResponseDTO?> RemoveCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartApiBaseUrl + "/api/cart/RemoveCart/"+cartDetailsId,
                Data = cartDetailsId
            });
        }

        public async Task<ResponseDTO?> GetCartItemsCountAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ShoppingCartApiBaseUrl + "/api/cart/GetCartItemsCount/" + userId
            });
        }
    }
}

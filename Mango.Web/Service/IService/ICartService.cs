using Mango.Web.Models.DTO;

namespace Mango.Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDTO?> GetCartByUserIdAsync(string userId);
        Task<ResponseDTO?> CartUpsertAsync(CartDTO cartDTO);
        Task<ResponseDTO?> RemoveCartAsync(int cartDetailsId);
        Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDTO);
        Task<ResponseDTO?> EmailCartAsync(CartDTO cartDTO);
    }
}

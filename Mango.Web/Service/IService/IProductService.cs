using Mango.Web.Models.DTO;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDTO?> GetAllProductsAsync();
        Task<ResponseDTO?> GetProductByIdAsync(int id);
        Task<ResponseDTO?> CreateProductAsync(ProductDTO couponDto);
        Task<ResponseDTO?> UpdateProductAsync(ProductDTO couponDto);
        Task<ResponseDTO?> DeleteProductAsync(int id);
    }
}

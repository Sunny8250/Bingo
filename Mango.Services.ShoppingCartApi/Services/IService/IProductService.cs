using Mango.Services.ShoppingCartApi.Models.DTO;

namespace Mango.Services.ShoppingCartApi.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync();
    }
}

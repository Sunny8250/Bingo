using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDTO?> CreateProductAsync(ProductDTO productDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ProductApiBaseUrl + "/api/product",
                Data = productDTO,
                ContentType = SD.ContentType.MultipartFormData
            });
        }

        public async Task<ResponseDTO?> DeleteProductAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProductApiBaseUrl + "/api/product/" + id,
            });
        }

        public async Task<ResponseDTO?> GetAllProductsAsync()
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductApiBaseUrl + "/api/product",
            });
        }

        public async Task<ResponseDTO?> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductApiBaseUrl + "/api/product/" + id,
            });
        }

        public async Task<ResponseDTO?> UpdateProductAsync(ProductDTO productDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.PUT,
                Url = SD.ProductApiBaseUrl + "/api/product",
                Data = productDTO,
				ContentType = SD.ContentType.MultipartFormData
			});
        }
    }
}

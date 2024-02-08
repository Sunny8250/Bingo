using Mango.Services.OrderApi.Models.DTO;
using Mango.Services.OrderApi.Services.IService;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;

namespace Mango.Services.OrderApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {

            _httpClientFactory = httpClientFactory;

        }
        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            HttpClient client = _httpClientFactory.CreateClient("Product");
            HttpResponseMessage? response = await client.GetAsync("/api/product");

            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if(resp!=null && resp.IsSuccess)
            {
                var products = JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(Convert.ToString(resp.Result));
                return products;
            }
            return new List<ProductDTO>();
        }
    }
}

using Mango.Web.Models.DTO;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDTO?> productDTOs = new();
            var response = await _productService.GetAllProductsAsync();
            if (response != null && response.IsSuccess)
            {
                productDTOs = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(productDTOs);
        }
        public async Task<IActionResult> ProductCreate(ProductDTO productDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProductAsync(productDTO);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product Created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }
            return View(productDTO);
        }

        public async Task<IActionResult> ProductDelete([FromRoute]int productID)
        {

            var response = await _productService.GetProductByIdAsync(productID);
            if (response != null && response.IsSuccess)
            {
                ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDTO productDTO)
        {

            var response = await _productService.DeleteProductAsync(productDTO.ProductID);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product Deleted successfully!";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(productDTO);
        }

        public async Task<IActionResult> ProductEdit([FromRoute]int productID)
        {

            var response = await _productService.GetProductByIdAsync(productID);
            if (response != null && response.IsSuccess)
            {
                ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDTO productDTO)
        {

            var response = await _productService.UpdateProductAsync(productDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product Update successfully!";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(productDTO);
        }
    }
}

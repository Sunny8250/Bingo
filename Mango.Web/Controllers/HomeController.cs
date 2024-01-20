using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
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

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productID)
        {
            ProductDTO? productDTO = new();
            var response = await _productService.GetProductByIdAsync(productID);
            if (response != null && response.IsSuccess)
            {
                productDTO = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(productDTO);
        }

        [HttpPost]
        [Authorize]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDTO productDTO)
        {
            CartDTO cartDTO = new()
            {
                CartHeader = new CartHeaderDTO()
                {
                    //we can also use JwtClaimTypes it is present in IdentityModel
                    UserID = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value,
                }
            };
            CartDetailsDTO cartDetailsDTO = new CartDetailsDTO()
            {
                ProductID = productDTO.ProductID,
                Count = productDTO.Count
            };
            List<CartDetailsDTO> cartDetailsDTOs = new List<CartDetailsDTO>() { cartDetailsDTO};
            cartDTO.CartDetails = cartDetailsDTOs;
            var response = await _cartService.CartUpsertAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product is added to cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(productDTO);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

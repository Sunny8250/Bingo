using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDTOBasedOnLoggedInUSer());
        }

        private async Task<CartDTO?> LoadCartDTOBasedOnLoggedInUSer()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            ResponseDTO? response = await _cartService.GetCartByUserIdAsync(userId);
            if(response!=null && response.IsSuccess)
            {
                CartDTO cartDto = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                return cartDto;
            }
            return new CartDTO();
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            ResponseDTO? responseDTO = await _cartService.RemoveCartAsync(cartDetailsId);
            if(responseDTO!=null && responseDTO.IsSuccess)
            {
                TempData["success"] = "Cart item removed";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO)
        {
            ResponseDTO? responseDTO = await _cartService.ApplyCouponAsync(cartDTO);
            if (responseDTO != null && responseDTO.IsSuccess)
            {
                if(!string.IsNullOrWhiteSpace(cartDTO.CartHeader.CouponCode))
                {
                    var cartResponse = await _cartService.GetCartByUserIdAsync(cartDTO.CartHeader.UserID);
                    if(cartResponse!=null && cartResponse.IsSuccess)
                    {
                        cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(cartResponse.Result));
                        TempData["success"] = "Coupon applied";
                    }
                    else
                    {
                        TempData["error"] = "Invalid Coupon";
                    }
                }
            }
            return RedirectToAction(nameof(CartIndex));
        }
    }
}

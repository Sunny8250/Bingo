using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.NetworkInformation;
using System.Reflection.PortableExecutable;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly ICouponService _couponService;
        public CartController(ICartService cartService, IOrderService orderService, ICouponService couponService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _couponService = couponService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDTOBasedOnLoggedInUSer());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDTOBasedOnLoggedInUSer());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartDTOBasedOnLoggedInUSer();
            cart.CartHeader.Phone = cartDTO.CartHeader.Phone;
            cart.CartHeader.Email = cartDTO.CartHeader.Email;
            cart.CartHeader.Name = cartDTO.CartHeader.Name;

            var response = await _orderService.CreateOrderAsync(cart);
            if(response!=null && response.IsSuccess)
            {
                OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(response.Result.ToString());
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                //get stripe session and redirect to stripe to place order
                StripeRequestDTO stripeSessionRequestDTO = new()
                {
                    ApprovedUrl = domain + "cart/confirmation?orderId=" + orderHeaderDTO.OrderHeaderID,
                    CancelUrl = domain + "cart/checkout",
                    OrderHeaderDTO = orderHeaderDTO
                };
                var stripeSessionResult = await _orderService.CreateStripeSessionAsync(stripeSessionRequestDTO);
                if(stripeSessionResult != null && stripeSessionResult.IsSuccess)
                {
                    var stripeSession = JsonConvert.DeserializeObject<StripeRequestDTO>(Convert.ToString(stripeSessionResult.Result));
                    //When a client sends a POST request to the server to create or update a resource, and the request is successful, 
                    //    the server may respond with a 303 status code along with a Location header pointing to the new location of the resource
                    Response.Headers.Add("Location", stripeSession.StripeSessionUrl);
                    return new StatusCodeResult(303);
                }
                else
                {
					TempData["error"] = "Please apply valid coupon";
				}
            }
            return View(cartDTO);
        }
        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDTO? responseDTO = await _orderService.ValidateStripeSessionAsync(orderId);
            if (responseDTO != null && responseDTO.IsSuccess)
            {
                OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(responseDTO.Result.ToString());
                if(orderHeaderDTO.Status == SD.Status_Approved)
                {
                    TempData["success"] = "Order Placed";
                    return View(orderId);
                }
            }
            //Within paymentIntent there are many status if not succeed than we can redirect to some error page based on the status
            return View(orderId);
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
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO, string couponCode=null)
        {
            if(couponCode!=null)
            {
                cartDTO.CartHeader.CouponCode = couponCode;
            }
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDTO);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
		public async Task<IActionResult> Coupons(string userId)
		{
			ResponseDTO? couponResponse = await _couponService.GetAllCouponsAsync();
            ResponseDTO? cartResponse = await _cartService.GetCartByUserIdAsync(userId);
            if (cartResponse != null & cartResponse.IsSuccess)
			{
                var coupons = JsonConvert.DeserializeObject<IEnumerable<CouponDTO>>(couponResponse.Result.ToString());
                var cart = JsonConvert.DeserializeObject<CartDTO>(cartResponse.Result.ToString());
                ViewBag.Coupons = coupons;
                return View(cart);
			}
			return RedirectToAction(nameof(CartIndex));
		}

		[HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartDTOBasedOnLoggedInUSer();
            cart.CartHeader.Email = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email)?.Value;
            ResponseDTO? response = await _cartService.EmailCartAsync(cart);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly!!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
        {
            cartDTO.CartHeader.CouponCode = "";
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDTO);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        private async Task<CartDTO?> LoadCartDTOBasedOnLoggedInUSer()
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            ResponseDTO? response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                CartDTO cartDto = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                if(!string.IsNullOrWhiteSpace(cartDto.CartHeader.CouponCode))
                {
                    var couponResponse = await _couponService.GetCouponAsync(cartDto.CartHeader.CouponCode);

                    if(couponResponse != null && couponResponse.IsSuccess)
                    {
						var coupon = JsonConvert.DeserializeObject<CouponDTO>(couponResponse.Result.ToString());
						if (!(cartDto.CartHeader.CartTotal >= coupon.MinAmount))
						{
							cartDto.CartHeader.CouponCode = string.Empty;
						}
					}
                }
                return cartDto;
            }
            return new CartDTO();
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            ////TODO
            //var userId = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            //var response = await _cartService.GetCartItemsCountAsync(userId);
            //int count = 0;
            //if (response !=null && response.IsSuccess)
            //{
            //    count = JsonConvert.DeserializeObject<int>(Convert.ToString(response.Result));

            //}
            //return Ok(count);

            var userId = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var response = await _cartService.GetCartByUserIdAsync(userId);
            int count = 0;
            if(response!=null && response.IsSuccess)
            {
                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                count = cartDTO.CartDetails.Count();
            }
            return Ok(count);
        }

    }
}

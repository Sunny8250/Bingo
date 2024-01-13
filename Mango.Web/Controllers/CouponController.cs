using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO?> couponDTOs = new();
            var response = await _couponService.GetAllCouponsAsync();
            if(response !=null && response.IsSuccess)
            {
                couponDTOs = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(couponDTOs);
        }

        public async Task<IActionResult> CouponCreate(CouponDTO couponDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _couponService.CreateCouponsAsync(couponDTO);
                if (response != null && response.IsSuccess)
                {
					TempData["success"] = "Coupon Created successfully";
					return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response.Message;
                }
            }
            return View(couponDTO);
        }
        
        public async Task<IActionResult> CouponDelete([FromQuery] int couponID)
        {
        
			var response = await _couponService.GetCouponByIdAsync(couponID);
			if (response != null && response.IsSuccess)
			{
				CouponDTO? model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                return View(model);
			}
            else
            {
                TempData["error"] = response.Message;
            }
            return NotFound();
		}
        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDTO couponDTO)
        {

            var response = await _couponService.DeleteCouponsAsync(couponDTO.CouponID);
            if (response != null && response.IsSuccess)
            {
				TempData["success"] = "Coupon Deleted successfully!";
				return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(couponDTO);
        }

    }
}

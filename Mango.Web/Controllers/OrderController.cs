using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        public IActionResult OrderIndex()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeaderDTO> list = new List<OrderHeaderDTO>();

            string? userId = "";
            if (!User.IsInRole(SD.AdminRole))
            {
                userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            ResponseDTO responseDTO = await _orderService.GetAllOrdersAsync(userId);
            if(responseDTO != null && responseDTO.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<IEnumerable<OrderHeaderDTO>>(responseDTO.Result.ToString());
                switch(status)
                {
                    case "approved":
                        list = list.Where(x => x.Status == SD.Status_Approved);
                        break;
                    case "readyforpickup":
                        list = list.Where(x => x.Status == SD.Status_ReadyForPickup);
                        break;
                    case "cancelled":
                        list = list.Where(x => x.Status == SD.Status_Cancelled);
                        break;
                    default: break;
                }
            }
            return Json(new { data = list });
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDTO orderHeaderDTO = new OrderHeaderDTO();
            string? userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO responseDTO = await _orderService.GetOrderByIdAsync(orderId);
            if (responseDTO != null && responseDTO.IsSuccess)
            {
                orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(responseDTO.Result.ToString());
            }
            if(!User.IsInRole(SD.AdminRole) && userId!=orderHeaderDTO.UserID)
            {
                return NotFound();
            }
            return View(orderHeaderDTO);
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            ResponseDTO responseDTO = await _orderService.UpdateOrderStatusAsync(orderId, SD.Status_ReadyForPickup);
            if (responseDTO != null && responseDTO.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }            
            return View();
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            ResponseDTO responseDTO = await _orderService.UpdateOrderStatusAsync(orderId, SD.Status_Completed);
            if (responseDTO != null && responseDTO.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            ResponseDTO responseDTO = await _orderService.UpdateOrderStatusAsync(orderId, SD.Status_Cancelled);
            if (responseDTO != null && responseDTO.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }
    }
}

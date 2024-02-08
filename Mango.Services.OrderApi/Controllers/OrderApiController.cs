using AutoMapper;
using Mango.Services.OrderApi.Data;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Models.DTO;
using Mango.Services.OrderApi.Services.IService;
using Mango.Services.OrderApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderApi.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderApiController : ControllerBase
    {
        private readonly ResponseDTO _responseDTO;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;

        public OrderApiController(IProductService productService, IMapper mapper, AppDbContext dbContext)
        {
            _responseDTO = new ResponseDTO();
            _productService = productService;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDTO)
        {
            try
            {

                //Exception column name full name
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = SD.Status_Pending;
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDTO.CartDetails);

                OrderHeader orderCreated = _dbContext.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDTO)).Entity;
                await _dbContext.SaveChangesAsync();

                orderHeaderDTO.OrderHeaderID = orderCreated.OrderHeaderID;
                _responseDTO.Result = orderHeaderDTO;               
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }
        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    Discounts = new List<SessionDiscountOptions>()
                };
                if(stripeRequestDTO.OrderHeaderDTO.Discount > 0)
                {
                    options.Discounts.Add(new SessionDiscountOptions { Coupon = stripeRequestDTO.OrderHeaderDTO.CouponCode });
                }
                foreach (var lineitem in stripeRequestDTO.OrderHeaderDTO.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(lineitem.Product.Price * 100),
                            Currency = "INR",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = lineitem.Product.Name
                            }
                        },
                        Quantity = lineitem.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new SessionService();
                Session session = service.Create(options); //its a stripe session
                stripeRequestDTO.StripeSessionUrl = session.Url; //here in this we can see in the url an unordered characters which is the session
                OrderHeader orderHeader = await _dbContext.OrderHeaders.FirstAsync(x => x.OrderHeaderID == stripeRequestDTO.OrderHeaderDTO.OrderHeaderID);
                orderHeader.StripeSessionID = session.Id;
                _dbContext.SaveChanges();
                _responseDTO.Result = stripeRequestDTO;
            }
            catch(Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }
        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDTO> ValidateStripeSession([FromBody]int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _dbContext.OrderHeaders.First(x => x.OrderHeaderID == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionID); //gets a stripe session
                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if(paymentIntent.Status == "succeeded")
                {
                    //than payment was succesful
                    orderHeader.PaymentIntentID = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    await _dbContext.SaveChangesAsync();
                    _responseDTO.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                };
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
            }
            return _responseDTO;
        }
    }
}

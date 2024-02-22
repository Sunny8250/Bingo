using AutoMapper;
using Mango.DB;
using Mango.MessageBus;
using Mango.Services.ShoppingCartApi.Data;
using Mango.Services.ShoppingCartApi.Models;
using Mango.Services.ShoppingCartApi.Models.DTO;
using Mango.Services.ShoppingCartApi.Services.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mango.Services.ShoppingCartApi.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartApiController : ControllerBase
    {
        private readonly ResponseDTO _responseDTO;
        private readonly IMapper _mapper;
        private readonly AppDbContext _dbContext;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private readonly DataService _dataService;
        public CartApiController(IMapper mapper, AppDbContext dbContext, IProductService productService,
            ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
        {
            _responseDTO = new ResponseDTO();
            _mapper = mapper;
            _dbContext = dbContext;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
            _dataService = new DataService(dbContext);
        }
        [HttpGet("GetCartById/{userId}")]
        public async Task<ResponseDTO> GetCartById(string userId)
        {
            try
            {
                CartDTO cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(_dbContext.CartHeaders.First(x => x.UserID == userId)),
                };

                // if admin removes any product, temp fix to handle from throwing exception.
                //start
				var products = await _productService.GetProductsAsync();
                var cartDetails = _dbContext.CartDetails.Where(x => x.CartHeaderID == cart.CartHeader.CartHeaderID).ToList();

                var joinCartDetailsWithProduct = from cd in cartDetails
                                                   join p in products
                                                   on cd.ProductID equals p.ProductID
                                                   select cd;
                //end
				cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(joinCartDetailsWithProduct);
                
                foreach (var item in cart.CartDetails)
                {
                    item.Product = products.FirstOrDefault(x => x.ProductID == item.ProductID);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);      
                }
                if(!string.IsNullOrWhiteSpace(cart.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCouponByCodeAsync(cart.CartHeader.CouponCode);
                    if(coupon!= null && cart.CartHeader.CartTotal >= coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                    else
                    {
                        cart.CartHeader.CouponCode = null;
                    }
                }
                _responseDTO.Result = cart;
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message.ToString();
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDTO> CartUpsert([FromBody]CartDTO cartDTO)
        {
            try
            {
                var cartHeaderResult = await _dbContext.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.UserID == cartDTO.CartHeader.UserID);
                //If cartHeader doesn't have any product details for the user.
                if(cartHeaderResult==null)
                {
                    //create Cart Header
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                    _dbContext.Add(cartHeader);
                    await _dbContext.SaveChangesAsync();

                    //Create Cart Details
                    cartDTO.CartDetails.First().CartHeaderID = cartHeader.CartHeaderID;
                    _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    //check if same user has details with same product
                    var cartDetailsResult = await _dbContext.CartDetails.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ProductID == cartDTO.CartDetails.First().ProductID &&
                        x.CartHeaderID == cartHeaderResult.CartHeaderID);
                    if(cartDetailsResult == null)
                    {
                        //Add Cart Details under same Cart Header Id
                        cartDTO.CartDetails.First().CartHeaderID = cartHeaderResult.CartHeaderID;
                        _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //Update Count in cart details
                        cartDTO.CartDetails.First().Count += cartDetailsResult.Count;
                        cartDTO.CartDetails.First().CartHeaderID = cartDetailsResult.CartHeaderID;
                        cartDTO.CartDetails.First().CartDetailsID = cartDetailsResult.CartDetailsID;
                        _dbContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                    }
                    
                }
                _responseDTO.Result = cartDTO;

            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message.ToString();
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }

        [HttpPost("RemoveCart/{cartDetailsId:int}")]
        public async Task<ResponseDTO> RemoveCart([FromRoute]int cartDetailsId)
        {
            try
            {
                var cartDetailsResult = _dbContext.CartDetails.First(x => x.CartDetailsID == cartDetailsId);
                int totalCartItemsCount = _dbContext.CartDetails.Count(x => x.CartHeaderID == cartDetailsResult.CartHeaderID);

                //CartDetails to remove
                _dbContext.CartDetails.Remove(cartDetailsResult);
                
                if (totalCartItemsCount == 1)
                {
                    //CartHeader to remove
                    var cartHeaderToRemove = await _dbContext.CartHeaders
                        .FirstOrDefaultAsync(x => x.CartHeaderID == cartDetailsResult.CartHeaderID);
                    _dbContext.CartHeaders.Remove(cartHeaderToRemove);


                }
                await _dbContext.SaveChangesAsync(); 
               
                _responseDTO.Result = true;

            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message.ToString();
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDTO> ApplyCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartHeaderResult = await _dbContext.CartHeaders.FirstAsync(x => x.UserID == cartDTO.CartHeader.UserID);
                cartHeaderResult.CouponCode = !string.IsNullOrWhiteSpace(cartDTO.CartHeader.CouponCode)
                    ? cartDTO.CartHeader.CouponCode : "";
                _dbContext.Update(cartHeaderResult);
                await _dbContext.SaveChangesAsync();
                _responseDTO.Result = true;
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message.ToString();
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }
        //endpoint to send message to serviceBus
        [HttpPost("EmailCartRequest")]
        public async Task<ResponseDTO> EmailCartRequest([FromBody] CartDTO cartDTO)
        {
            try
            {
                var queueName = _configuration.GetValue<string>("TopicAndQueueName:EmailShoppingCartQueue");
                await _messageBus.PublishMessage(cartDTO, queueName);
                _responseDTO.Result = true;
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message.ToString();
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }

        //TODO
        [HttpGet("GetCartItemsCount/{userId}")]
        public async Task<ResponseDTO> GetCartItemsCount(string userId)
        {
            try
            {
                var queryparam = new SqlParameter("@userId", userId);
                var products = await _dataService.Execute("GetCartItemsCount", queryparam, CommandType.StoredProcedure);

                // Extract the ProductId column from the DataTable
                var productIds = products.AsEnumerable()
                                           .Select(row => row.Field<int>("ProductId"))
                                           .ToList();
                _responseDTO.Result = productIds.Count;
            }
            catch(Exception ex)
            {
                _responseDTO.Message = ex.Message.ToString();
                _responseDTO.IsSuccess = false;
            }
            return _responseDTO;
        }
    }
}

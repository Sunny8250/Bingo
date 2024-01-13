using AutoMapper;
using Mango.Services.CouponApi.CustomActionFilter;
using Mango.Services.CouponApi.Data;
using Mango.Services.CouponApi.Models;
using Mango.Services.CouponApi.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponApi.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponApiController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ResponseDTO _response;
        private readonly IMapper _mapper;
        public CouponApiController(AppDbContext dbContext, ResponseDTO response, IMapper mapper)
        {
            _dbContext = dbContext;
            _response = response;
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDTO Get()
        {
            try
            {
                var coupons = _dbContext.Coupons.ToList();
                
                _response.Result = _mapper.Map<List<CouponDTO>>(coupons);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDTO GetById([FromRoute] int id)
        {
            try
            {
                var coupon = _dbContext.Coupons.First(x => x.CouponID == id);
                
                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{code}")]
        public ResponseDTO GetByCode([FromRoute] string code)
        {
            try
            {
                var coupon = _dbContext.Coupons.First(x => x.CouponCode == code);
                
                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpPost]
        [ValidateModelAttribute]
        public ResponseDTO Create([FromBody] AddCouponDTO addCouponDTO)
        {
            try
            {
                var coupon = _mapper.Map<Coupon>(addCouponDTO);
                _dbContext.Coupons.Add(coupon);
                _dbContext.SaveChanges();
                
                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpPut]
        [ValidateModelAttribute]
        public ResponseDTO Update([FromBody] UpdateCouponDTO updateCouponDTO)
        {
            try
            {
                var coupon = _mapper.Map<Coupon>(updateCouponDTO);
                _dbContext.Coupons.Update(coupon);
                _dbContext.SaveChanges();
                
                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        public ResponseDTO Delete([FromRoute] int id)
        {
            try
            {
                var coupon = _dbContext.Coupons.First(x => x.CouponID == id);
                _dbContext.Coupons.Remove(coupon);
                _dbContext.SaveChanges();
                var couponDTO = _mapper.Map<CouponDTO>(coupon);
                _response.Result = couponDTO;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}

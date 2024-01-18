using AutoMapper;
using Azure;
using Mango.Services.ProductApi.Data;
using Mango.Services.ProductApi.Models;
using Mango.Services.ProductApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductApi.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ResponseDTO _response;
        private readonly IMapper _mapper;
        public ProductApiController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _response = new ResponseDTO();
        }

        [HttpGet]
        public ResponseDTO Get()
        {
            try
            {
                var products = _dbContext.Products.ToList();
                _response.Result = _mapper.Map<List<ProductDTO>>(products);
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
                var product = _dbContext.Products.First(x => x.ProductID == id);

                _response.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ResponseDTO Create(AddProductDTO addProductDTO)
        {
            try
            {
                var product = _mapper.Map<Product>(addProductDTO);
                _dbContext.Products.Add(product);
                _dbContext.SaveChanges();

                _response.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
		public ResponseDTO Update([FromBody] UpdateProductDTO updateProductDTO)
        {
            try
            {
                var product = _mapper.Map<Product>(updateProductDTO);
                _dbContext.Products.Update(product);
                _dbContext.SaveChanges();

                _response.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
		[Route("{id:int}")]
		public ResponseDTO Delete([FromRoute] int id)
        {
            try
            {
                var product = _dbContext.Products.First(x => x.ProductID == id);
                _dbContext.Products.Remove(product);
                _dbContext.SaveChanges();
                var couponDTO = _mapper.Map<ProductDTO>(product);
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

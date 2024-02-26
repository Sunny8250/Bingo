using AutoMapper;
using Azure;
using Mango.Services.ProductApi.Data;
using Mango.Services.ProductApi.Models;
using Mango.Services.ProductApi.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace Mango.Services.ProductApi.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ResponseDTO _response;
        private readonly IMapper _mapper;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ProductApiController(AppDbContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
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
        public ResponseDTO Create(AddProductDTO addProductDTO) //Here removed FromBody due to data was coming from body but got appended
        {
            try
            {
                var product = _mapper.Map<Product>(addProductDTO);
                _dbContext.Products.Add(product);
                _dbContext.SaveChanges();

                if(addProductDTO.Image != null)
                {
                    var fileName = addProductDTO.Image.FileName;
                    var fileExtension = Path.GetExtension(addProductDTO.Image.FileName); 
					var localFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", $"{fileName}{fileExtension}");
					//Reads the FileStream for this localPath and knows to create a file
					//Upload image to localPath
					using var stream = new FileStream(localFilePath, FileMode.Create);
					addProductDTO.Image.CopyTo(stream);

					//Here we cannot just simply pass path like C:\users\.... instead
					//https://localhost:1234/images/filename.jpg here https is the scheme localhost is the host and port is the pathbase

					var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{fileName}{fileExtension}";
					product.ImageUrl = urlFilePath;
                    product.ImageLocalPath = localFilePath;
                }
                else
                {
                    product.ImageUrl = "https://placeholder.co/600*400";
                }
                _dbContext.Update(product);
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
		public ResponseDTO Update(UpdateProductDTO updateProductDTO)
        {
            try
            {
                var product = _mapper.Map<Product>(updateProductDTO);

				if (!string.IsNullOrEmpty(product.ImageLocalPath))
				{
					var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
					FileInfo file = new FileInfo(oldFilePath);
					if (file.Exists)
					{
						file.Delete();
					}
				}

				if (updateProductDTO.Image != null)
				{
					var fileName = updateProductDTO.Image.FileName;
					var fileExtension = Path.GetExtension(updateProductDTO.Image.FileName);
					var localFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", $"{fileName}{fileExtension}");
					//Reads the FileStream for this localPath and knows to create a file
					//Upload image to localPath
					using var stream = new FileStream(localFilePath, FileMode.Create);
					updateProductDTO.Image.CopyToAsync(stream);

					//Here we cannot just simply pass path like C:\users\.... instead
					//https://localhost:1234/images/filename.jpg here https is the scheme localhost is the host and port is the pathbase

					var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Images/{fileName}{fileExtension}";
					product.ImageUrl = urlFilePath;
					product.ImageLocalPath = localFilePath;
				}


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
                if(!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePath);
                    if(file.Exists)
                    {
                        file.Delete();
                    }
                }
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

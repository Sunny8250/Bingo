using System.ComponentModel.DataAnnotations;

namespace Mango.Services.ProductApi.Models.DTO
{
    public class UpdateProductDTO
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
		public string? ImageUrl { get; set; }
		public string? ImageLocalPath { get; set; }
		public IFormFile? Image { get; set; }
	}
}

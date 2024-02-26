using Mango.Web.Utility;
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models.DTO
{
    public class ProductDTO
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
		public string? ImageUrl { get; set; }
		public string? ImageLocalPath { get; set; }
        [MaximumFileSize(1)]
        [AllowedExtensions(new string[] {".jpg", ".jpeg", ".png"})]
        public IFormFile? Image { get; set; }
		public int Count { get; set; } = 1;
    }
}

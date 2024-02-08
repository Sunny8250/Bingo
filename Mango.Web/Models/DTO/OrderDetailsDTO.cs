
namespace Mango.Web.Models.DTO
{
    public class OrderDetailsDTO
    {
        public int OrderDetailsID { get; set; }
        public int OrderHeaderID { get; set; }
        public int ProductID { get; set; }
        public ProductDTO? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}

using Mango.Services.OrderApi.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderApi.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsID { get; set; }
        [ForeignKey("OrderHeaderID")]
        public int OrderHeaderID { get; set; }
        public OrderHeader? OrderHeader { get; set; }
        public int ProductID { get; set; }
        [NotMapped]
        public ProductDTO? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
    }
}

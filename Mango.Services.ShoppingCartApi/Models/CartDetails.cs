using Mango.Services.ShoppingCartApi.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartApi.Models
{
    public class CartDetails
    {
        [Key]
        public int CartDetailsID { get; set; }
        public int CartHeaderID { get; set; }
        [ForeignKey("CartHeaderID")]
        public CartHeader CartHeader { get; set; }
        public int ProductID { get; set; }
        [NotMapped]
        public ProductDTO Product { get; set; }
        public int Count { get; set; }

    }
}

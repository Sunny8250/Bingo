using System.ComponentModel.DataAnnotations;

namespace Mango.Services.CouponApi.Models.DTO
{
    public class AddCouponDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "Minimum 3 chararcters")]
        public string CouponCode { get; set; }
        [Required]
        public double DiscountAmount { get; set; }
        [Required]
        public int MinAmount { get; set; }
    }
}

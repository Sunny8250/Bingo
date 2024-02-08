using System.ComponentModel.DataAnnotations;

namespace Mango.Services.OrderApi.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderID { get; set; }
        public string? UserID { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double OrderTotal { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime OrderTime { get; set; }
        public string? Status { get; set; }
        public string? PaymentIntentID { get; set; }
        public string? StripeSessionID { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }

    }
}

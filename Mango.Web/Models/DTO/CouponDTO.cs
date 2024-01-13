namespace Mango.Web.Models.DTO
{
    public class CouponDTO
    {
        public int CouponID { get; set; }
        public string CouponCode { get; set; }
        public double DiscountAmount { get; set; }
        public int MinAmount { get; set; }
    }
}

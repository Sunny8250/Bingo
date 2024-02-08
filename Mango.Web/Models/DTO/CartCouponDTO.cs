namespace Mango.Web.Models.DTO
{
    public class CartCouponDTO
    {
        public CartDTO Cart { get; set; }
        public IEnumerable<CouponDTO> Coupons { get; set; }
    }
}

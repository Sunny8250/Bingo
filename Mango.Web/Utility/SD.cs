namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponApiBaseUrl { get; set; }
        public static string AuthApiBaseUrl { get; set; }
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}

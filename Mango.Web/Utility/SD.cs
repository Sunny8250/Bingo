namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponApiBaseUrl { get; set; }
        public static string AuthApiBaseUrl { get; set; }

        public const string TokenCookie = "JwtToken";
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}

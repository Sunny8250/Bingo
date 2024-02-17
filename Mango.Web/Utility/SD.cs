namespace Mango.Web.Utility
{
    public class SD
    {
        public static string CouponApiBaseUrl { get; set; }
        public static string AuthApiBaseUrl { get; set; }
        public static string ProductApiBaseUrl { get; set; }
        public static string ShoppingCartApiBaseUrl { get; set; }
        public static string OrderApiBaseUrl { get; set; }

        public const string TokenCookie = "JwtToken";
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadyForPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Refunded = "Refunded";
        public const string Status_Cancelled = "Cancelled";

        public const string RoleAdmin = "Admin";
        public const string RoleCustomer = "Customer";


        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}

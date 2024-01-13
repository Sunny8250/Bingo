namespace Mango.Services.CouponApi.Models.DTO
{
    public class ResponseDTO
    {
        public object? Result { get; set; }
        public bool IsSuccess { get; set; } = true;
        public String Message { get; set; } = "";
    }
}

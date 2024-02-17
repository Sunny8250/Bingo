namespace Mango.Services.OrderApi.Models.DTO
{
	public class RewardsDTO
	{
		public string UserID { get; set; }
		public int RewardsActivity { get; set; }
		public int OrderId { get; set; }
        public string? Email { get; set; }
    }
}

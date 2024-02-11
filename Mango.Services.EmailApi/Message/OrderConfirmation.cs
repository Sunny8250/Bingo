namespace Mango.Services.EmailApi.Message
{
	public class OrderConfirmation
	{
		public string UserID { get; set; }
		public int RewardsActivity { get; set; }
		public int OrderId { get; set; }
	}
}

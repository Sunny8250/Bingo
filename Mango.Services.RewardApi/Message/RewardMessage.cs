namespace Mango.Services.RewardApi.Message
{
	public class RewardMessage
	{
		public string UserID { get; set; }
		public int RewardsActivity { get; set; }
		public int OrderId { get; set; }
	}
}

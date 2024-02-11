using Mango.Services.RewardApi.Data;
using Mango.Services.RewardApi.Models;
using Mango.Services.RewardApi.Message;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Mango.Services.RewardApi.Services;

namespace Mango.Services.RewardApi.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public RewardService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        //We cannot use registered DbContext here because dbContext is Scoped and EmailService is singleton, so to resolve this
        //we should create a another dbcontext where it will be singleton


		public async Task UpdateRewards(RewardMessage rewardMessage)
		{
            try
            {
                Rewards rewards = new()
                {
                    OrderId = rewardMessage.OrderId,
                    RewardsActivity = rewardMessage.RewardsActivity,
                    UserId = rewardMessage.UserID,
                    RewardsDate = DateTime.Now
                };
				await using (var dbContext = new AppDbContext(_dbOptions))
				{
					await dbContext.Rewards.AddAsync(rewards);
					await dbContext.SaveChangesAsync();
					await dbContext.DisposeAsync();
				}
			}
            catch (Exception ex) { }
		}
	}
}

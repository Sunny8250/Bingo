using Mango.Services.RewardApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Mango.Services.RewardApi.Data
{
	public class AppDbContext: DbContext
	{
		public AppDbContext(DbContextOptions options): base(options)
		{
			
		}
		public DbSet<Rewards> Rewards { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
		}
	}
}

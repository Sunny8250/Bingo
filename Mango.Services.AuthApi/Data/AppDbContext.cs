using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Mango.Services.AuthApi.Models;

namespace Mango.Services.AuthApi.Data
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
	{
        public AppDbContext(DbContextOptions options): base(options)
        {
            
        }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
		}
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    }
}

using Mango.Services.CouponApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Mango.Services.CouponApi.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions options): base(options)
        {
            
        }
        public DbSet<Coupon> Coupons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var coupons = new List<Coupon>();
            coupons.Add(new Coupon
            {
                CouponID = 1,
                CouponCode = "100FF",
                DiscountAmount = 10,
                MinAmount = 50
            });
            coupons.Add(new Coupon
            {
                CouponID = 2,
                CouponCode = "200FF",
                DiscountAmount = 15,
                MinAmount = 60
            });
            modelBuilder.Entity<Coupon>().HasData(coupons);
        }
    }
}

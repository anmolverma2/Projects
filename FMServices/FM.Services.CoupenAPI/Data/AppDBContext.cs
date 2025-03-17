using FM.Services.CoupenAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FM.Services.CoupenAPI.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
        {
            
        }
        public DbSet<CouponModel> Coupens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CouponModel>().HasData(
               new List<CouponModel>
               {
                   new CouponModel{
                       CouponId = 1,
                       CoupenCode = "10OFF",
                       DiscountAmount = 10,
                       MinAmount = 20
                   },
                   new CouponModel
                   {
                       CouponId = 2,
                       CoupenCode = "20OFF",
                       DiscountAmount = 20,
                       MinAmount = 40
                   }
               });
        }

    }
}

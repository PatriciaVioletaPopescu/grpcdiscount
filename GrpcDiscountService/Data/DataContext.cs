using GrpcDiscountService.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcDiscountService.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<Discount> Discounts => Set<Discount>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Discount>()
                .HasIndex(discount => discount.Code)
                .IsUnique();
        }
    }
}

using devops_cart_service.Models;
using Microsoft.EntityFrameworkCore;

namespace devops_cart_service.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<CartOverview> CartOverviews { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartOverview>()
                .HasKey(c => c.CartId);

            modelBuilder.Entity<CartProduct>()
                .HasKey(c => c.CartProductId);

            base.OnModelCreating(modelBuilder);
        }
    }
}

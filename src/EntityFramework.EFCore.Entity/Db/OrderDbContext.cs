using Microsoft.EntityFrameworkCore;

namespace EntityFramework.EFCore.Entity.Db
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext()
        {
        }

        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Orders> Orders { get; set; }

        public virtual DbSet<Product> Product { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Orders>().HasKey(d => d.Id);
            modelBuilder.Entity<Product>().HasKey(d => d.Id);
        }
    }
}
using CustomerOrders.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerOrders.Infrastructure.Data
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options):base(options)
        {

        }
        public DbSet<CustomerOrders.Core.Entities.Customer> Customers { get; set; }
        public DbSet<CustomerOrders.Core.Entities.Product> Products { get; set; }
        public DbSet<CustomerOrders.Core.Entities.OrderItem> OrderItems { get; set; }
        public DbSet<CustomerOrders.Core.Entities.CustomerOrders> CustomerOrders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // OrderItem ve CustomerOrders arasındaki iliskisini tanımladım
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.CustomerOrder)
                .WithMany(co => co.OrderItems)
                .HasForeignKey(oi => oi.CustomerOrderId)
                .OnDelete(DeleteBehavior.Cascade); // Sipariş silindiğinde, ilgili urunleri de sildim

            // OrderItem ve Product arasındaskı ilişkiyi tanımladim
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Ürün silinirse, siparislerdeki ürün referanslarını korumamıgzerekır
        }
    }
}

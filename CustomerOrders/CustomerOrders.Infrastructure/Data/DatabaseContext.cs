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
        public DbSet<CustomerOrders.Core.Entities.OrderProduct> OrderProducts { get; set; }
        public DbSet<CustomerOrders.Core.Entities.CustomerOrders> CustomerOrders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // OrderProduct ve CustomerOrders arasındaki ilişkiyi tanımlıyorum
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Order)  // OrderProduct, CustomerOrder ile ilişkili
                .WithMany(co => co.OrderProducts)  // CustomerOrder, birden fazla OrderProduct içerebilir
                .HasForeignKey(op => op.OrderProductId)  // ForeignKey: CustomerOrderId
                .OnDelete(DeleteBehavior.Cascade);  // Sipariş silindiğinde, ilgili OrderProduct'ları da sil

            // OrderProduct ve Product arasındaki ilişkiyi tanımlıyorum
            modelBuilder.Entity<OrderProduct>()
                .HasOne(op => op.Product)  // OrderProduct, Product ile ilişkili
                .WithMany()  // Product, birden fazla OrderProduct ile ilişkilendirilebilir (sadece tek yönlü ilişki)
                .HasForeignKey(op => op.OrderProductId)  // ForeignKey: ProductId
                .OnDelete(DeleteBehavior.Restrict);  // Ürün silindiğinde, OrderProduct ilişkisini korumak için RESTRICT kullanıyorum
        }
    }
}

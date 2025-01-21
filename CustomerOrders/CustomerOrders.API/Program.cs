using CustomerOrders.API;
using CustomerOrders.Infrastructure.Data;
using Logging.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        var host = CreateHostBuilder(args).Build();
        await CreateAndSeedDb(host);
        host.Run();
    }
    private static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)

            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(LoggerService.CreateLogger());
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    private static async Task CreateAndSeedDb(IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = services.GetRequiredService<DatabaseContext>();
                context.Database.Migrate();
                await SeedDatabase(context);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError($"Exception occured in API: {ex.Message}");
            }
        }

    }
    private static async Task SeedDatabase(DatabaseContext context)
    {
        try
        {
            if (!context.Customers.Any())
            {
                var customers = new List<CustomerOrders.Core.Entities.Customer>
                {
                    new CustomerOrders.Core.Entities.Customer
                    {
                        Name = "selen",
                        Surname = "duman",
                        Email = "selen.doe@example.com",
                        PostingAddress = "�stanbul",
                        PhoneNumber = "05515636363",
                        WorkAddress = "istanbul",
                        Description = "deneme2"
                    },
                    new CustomerOrders.Core.Entities.Customer
                    {
                        Name = "elif",
                        Surname = "duman",
                        Email = "elif.doe@example.com",
                        PostingAddress = "�stanbul",
                        PhoneNumber = "05515636363",
                        WorkAddress = "istanbul",
                        Description = "deneme"
                    }
                };
                await context.Customers.AddRangeAsync(customers);
                await context.SaveChangesAsync();
            };

            if (!context.Products.Any())
            {
                var products = new List<CustomerOrders.Core.Entities.Product>
                {
                    new CustomerOrders.Core.Entities.Product
                    {
                        Name = "tv",
                        Description = "televizyon",
                        Price = 100,
                        Quanttity = 5,
                        Barcode = "SAMSUNG12562",
                    },
                    new CustomerOrders.Core.Entities.Product
                    {
                         Name = "kitap",
                         Description = "korku roman",
                         Price = 100,
                         Quanttity = 5,
                         Barcode = "BOOKS12562",
                    },
                    new CustomerOrders.Core.Entities.Product
                    {
                         Name = "buzdolab�",
                         Description = "korku roman",
                         Price = 100,
                         Quanttity = 5,
                         Barcode = "BUZS12562",
                    }
                };
                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
            if (!context.CustomerOrders.Any())
            {
                if (!context.CustomerOrders.Any())
                {
                    var customer1 = await context.Customers.FirstOrDefaultAsync(c => c.Name == "selen");
                    var customer2 = await context.Customers.FirstOrDefaultAsync(c => c.Name == "elif");
                    var product1 = await context.Products.FirstOrDefaultAsync(c => c.Name == "kitap");
                    var product2 = await context.Products.FirstOrDefaultAsync(c => c.Name == "tv");
                    var product3 = await context.Products.FirstOrDefaultAsync(c => c.Name == "buzdolab�");

                    if (customer1 != null && customer2 != null && product1 != null && product2 != null && product3 != null)
                    {
                        // Sipari�leri (CustomerOrders) ekleyelim
                        var customerOrders = new List<CustomerOrders.Core.Entities.CustomerOrders>
                        {
                            //new ustomerOrders.Core.Entities.CustomerOrders
                            //{
                            //    CustomerId = customer1.Id,
                            //    Status = 1, // Sipari� durumu: 1 (�rnek)
                            //    Description = "�lk sipari�",
                            //    OrderItems = new List<OrderItem>
                            //    {
                            //        new OrderItem { ProductId = product1.Id }, // �r�n 1
                            //        new OrderItem { ProductId = product2.Id }  // �r�n 2
                            //    }
                            //},
                            //new CustomerOrders.Core.Entities.CustomerOrders
                            //{
                            //    CustomerId = customer1.Id,
                            //    OrderProducts = new List<OrderProduct>
                            //    {
                            //        new OrderItem { ProductId = product2.Id }  // �r�n 2
                            //    }
                            //},
                            //new CustomerOrders
                            //{
                            //    CustomerId = customer2.Id,
                            //    Status = 1, // Sipari� durumu: 1 (�rnek)
                            //    Description = "���nc� sipari�",
                            //    OrderItems = new List<OrderItem>
                            //    {
                            //        new OrderProduct { p = product3.Id }  // �r�n 3
                            //    }
                            //}
                        };
                        await context.CustomerOrders.AddRangeAsync(customerOrders);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        catch (Exception ex)
        {

            throw ex;
        }
    }
}
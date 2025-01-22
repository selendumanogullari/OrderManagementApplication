using CustomerOrders.API;
using Authentication.Helpers;
using CustomerOrders.Core.Entities;
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
            if (!context.Users.Any())
            {
                var user = new CustomerOrders.Core.Entities.User
                {
                    Username = "selen",
                    PasswordHash = SSHA256Helper.HashPassword("123456"), // Þifreyi hashleyerek ekliyoruz
                };

                context.Users.Add(user);
                context.SaveChanges();
            }

            if (!context.Customers.Any())
            {
                var customers = new List<CustomerOrders.Core.Entities.Customer>
                {
                    new CustomerOrders.Core.Entities.Customer
                    {
                        Name = "selen",
                        Surname = "duman",
                        Email = "selen.doe@example.com",
                        PostingAddress = "Ýstanbul",
                        PhoneNumber = "05515636363",
                        WorkAddress = "istanbul",
                        Description = "deneme2"
                    },
                    new CustomerOrders.Core.Entities.Customer
                    {
                        Name = "elif",
                        Surname = "duman",
                        Email = "elif.doe@example.com",
                        PostingAddress = "Ýstanbul",
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
                        Price = 78,
                        Quanttity = 5,
                        Barcode = "SAMSUNG12562",
                    },
                    new CustomerOrders.Core.Entities.Product
                    {
                         Name = "kitap",
                         Description = "korku roman",
                         Price = 24,
                         Quanttity = 5,
                         Barcode = "BOOKS12562",
                    },
                    new CustomerOrders.Core.Entities.Product
                    {
                         Name = "buzdolabý",
                         Description = "korku roman",
                         Price = 1300,
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
                    var product3 = await context.Products.FirstOrDefaultAsync(c => c.Name == "buzdolabý");

                    if (customer1 != null && customer2 != null && product1 != null && product2 != null && product3 != null)
                    {
                        // Sipariþleri (CustomerOrders) ekleyelim
                        var customerOrders = new List<CustomerOrders.Core.Entities.CustomerOrders>
                        {
                           new CustomerOrders.Core.Entities.CustomerOrders
                            {
                                CustomerId = customer1.Id,
                                Status = 1, // Sipariþ durumu: 1 (Örnek)
                                Description = "Ýlk sipariþ",
                                OrderProducts = new List<OrderProduct>
                                {
                                    new OrderProduct { ProductId = product1.Id, Quantity = 1 }, // Ürün 1, Miktar 1
                                    new OrderProduct { ProductId = product2.Id, Quantity = 2 }  // Ürün 2, Miktar 2
                                },
                                TotalAmount = (product1.Price * 1) + (product2.Price * 2) // Ürünlerin fiyatlarýný miktarlarý ile çarpýp topluyoruz
                            },
                            new CustomerOrders.Core.Entities.CustomerOrders
                            {
                                CustomerId = customer1.Id,
                                Status = 1, // Sipariþ durumu: 1 (Örnek)
                                Description = "Ýkinci sipariþ",
                                OrderProducts = new List<OrderProduct>
                                {
                                    new OrderProduct { ProductId = product2.Id, Quantity = 1 }  // Ürün 2, Miktar 1
                                },
                                TotalAmount = product2.Price * 1 // Ürünün fiyatýný miktar ile çarpýyoruz
                            },
                            new CustomerOrders.Core.Entities.CustomerOrders
                            {
                                CustomerId = customer2.Id,
                                Status = 1, // Sipariþ durumu: 1 (Örnek)
                                Description = "Üçüncü sipariþ",
                                OrderProducts = new List<OrderProduct>
                                {
                                    new OrderProduct { ProductId = product3.Id, Quantity = 2 }  // Ürün 3, Miktar 2
                                },
                                TotalAmount = product3.Price * 2 // Ürünün fiyatýný miktar ile çarpýyoruz
                            }
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
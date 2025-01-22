using Authentication.Interfaces;
using Authentication.Services;
using CustomerOrders.Application.CustomerOrder.Handlers;
using CustomerOrders.Application.RabbitMQ.Interfaces;
using CustomerOrders.Core.Repositories;
using CustomerOrders.Infrastructure.Data;
using CustomerOrders.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Redis.Interfaces;
using Redis.Services;
using System.Reflection;
using System.Text;
using static CustomerOrders.Core.Repositories.Base.IRepository;

namespace CustomerOrders.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            services.AddControllers();

            var redisCacheUrl = Configuration["RedisCacheUrl"];

            if (string.IsNullOrEmpty(redisCacheUrl))
            {
                throw new Exception("RedisCacheUrl appsettings.json içinde tanımlanmalı.");
            }

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(redisCacheUrl, true);
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddApiVersioning();

            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CustomerConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly("CustomerOrders.API")),
                 ServiceLifetime.Scoped);

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse(Configuration["RedisCacheUrl"]);
                configuration.AbortOnConnectFail = false;
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddSingleton<Redis.Interfaces.ICacheService, Redis.Services.RedisCacheService>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration["RedisCacheUrl"];
            });

            services.AddSwaggerGen(c =>
            {
                try
                {
                    var executingAssembly = Assembly.GetExecutingAssembly();

                    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{executingAssembly.GetName().Name}.xml"));

                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderManagementApplication API", Version = "v1" });
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter JWT token with Bearer format like: Bearer [token]"
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {{
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }});

                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });


            services.AddAutoMapper(typeof(Startup));
            services.AddMediatR(typeof(UpdateCustomerOrdersCommandHandler).GetTypeInfo().Assembly);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ICustomerOrdersRepository, CustomerOrdersRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            //services.AddSingleton<IAuthService, AuthService>();
            services.AddScoped<IAuthService, AuthService>(); // Yeni kod

            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IRabbitMQProducer, RabbitMQProducer>();
            services.AddSingleton<RabbitMQConsumer>();

            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["AppSettings:ValidIssuer"],
                    ValidAudience = Configuration["AppSettings:ValidAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AppSettings:Secret"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });
            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerOrders.API  V1");
            });
        }
    }
}

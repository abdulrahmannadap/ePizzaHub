using ePizzaHub.Core.Database;
using ePizzaHub.Core.Database.Entities;
using ePizzaHub.Repositories.Impelmentations;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Implementations;
using ePizzaHub.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ePizzaHub.Services
{
    public class ConfigureServices
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
           services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DbConnection"));
            });


            //repositories
            services.AddScoped<IRepository<Item>, Repository<Item>>();
            services.AddScoped<IRepository<CartItem>, Repository<CartItem>>();
            services.AddScoped<IRepository<PaymentDetail>, Repository<PaymentDetail>>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICartRepository, CartRepository>();

            //services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IPaymentService, PaymentService>();
        }
    }
}

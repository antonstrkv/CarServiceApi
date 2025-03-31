using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess
{
    public static class Extensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICarRepository, CarRepository>();
            serviceCollection.AddDbContext<AppContext>(x =>
            {
                x.UseNpgsql("Host=localhost;Database=CarRental;Username=postgres;Password=1234");
            });
            return serviceCollection; 
        }
    }
}

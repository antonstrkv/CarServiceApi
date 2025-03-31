using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogic
{
    public static class Extentions
    {
        public static IServiceCollection AddBussinessLogic( this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ICarService, CarService>();
            return serviceCollection;
        }
    }
}

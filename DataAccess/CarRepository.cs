using DataAccess.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DataAccess
{
    internal class CarRepository : ICarRepository
    {
        private readonly AppContext context;
        private readonly IDistributedCache cache;
        private readonly RetryService retryService;
        private const string AvailableCarsCacheKey = "available_cars";

        public CarRepository(AppContext context, IDistributedCache distributedCache, RetryService retryService)
        {
            this.context = context;
            this.cache = distributedCache;
            this.retryService = retryService;
        }

        public async Task CreateAsync(Car car, CancellationToken cancellationToken = default)
        {
            await context.Cars.AddAsync(car, cancellationToken);
            await retryService.DoWithRetryAsync(
               () => context.SaveChangesAsync(cancellationToken),
               TimeSpan.FromSeconds(3)
           );
            await InvalidateCacheAsync(AvailableCarsCacheKey, cancellationToken);
        }

        public async Task<Car?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            string key = GetCarCacheKey(id);
            return await GetOrSetCacheAsync(
                key,
                async () => await context.Cars.FindAsync(new object[] { id }, cancellationToken),
                cancellationToken
            );
        }

        public async Task UpdateAsync(Car car, CancellationToken cancellationToken = default)
        {
            context.Cars.Update(car);
            await retryService.DoWithRetryAsync(
                () => context.SaveChangesAsync(cancellationToken),
                TimeSpan.FromSeconds(3)
            );

            await SetCacheAsync(GetCarCacheKey(car.Id), car, cancellationToken);
            await InvalidateCacheAsync(AvailableCarsCacheKey, cancellationToken);
        }

        public async Task<IEnumerable<Car>> GetAvailableCarsAsync(CancellationToken cancellationToken = default)
        {
            return await GetOrSetCacheAsync(
                AvailableCarsCacheKey,
                async () => await context.Cars.Where(c => c.IsAvailable).ToListAsync(cancellationToken),
                cancellationToken
            );
        }

        public async Task<bool> ReturnCarAsync(int id, CancellationToken cancellationToken = default)
        {
            var car = await context.Cars.FindAsync(new object[] { id }, cancellationToken);
            if (car is null || car.IsAvailable)
                return false;

            car.IsAvailable = true;
            context.Cars.Update(car);
            await retryService.DoWithRetryAsync(
                () => context.SaveChangesAsync(cancellationToken),
                TimeSpan.FromSeconds(3)
            );

            await SetCacheAsync(GetCarCacheKey(id), car, cancellationToken);
            await InvalidateCacheAsync(AvailableCarsCacheKey, cancellationToken);
            return true;
        }

        //  SUPPORTIVE METHODS 

        private string GetCarCacheKey(int id) => $"car:{id}";

        private async Task<T?> GetOrSetCacheAsync<T>(string key, Func<Task<T>> factory, CancellationToken cancellationToken)
        {
            var cachedString = await cache.GetStringAsync(key, cancellationToken);
            if (cachedString != null)
            {
                Console.WriteLine($"{key} retrieved from cache");
                return JsonSerializer.Deserialize<T>(cachedString);
            }

            var result = await factory();
            if (result is not null)
            {
                await SetCacheAsync(key, result, cancellationToken);
                Console.WriteLine($"{key} retrieved from DB and cached");
            }

            return result;
        }

        private async Task SetCacheAsync<T>(string key, T value, CancellationToken cancellationToken)
        {
            var serialized = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            }, cancellationToken);
        }

        private async Task InvalidateCacheAsync(string key, CancellationToken cancellationToken)
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
    }
}

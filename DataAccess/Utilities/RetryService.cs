using Microsoft.Extensions.Configuration;

namespace DataAccess.Utilities
{
    public class RetryService
    {
        private readonly int tryCount;

        public RetryService(IConfiguration configuration)
        {
            tryCount = int.TryParse(configuration["RetrySettings:TryCount"], out int count) && count > 0
                ? count
                : 3; 
        }

        public async Task DoWithRetryAsync(Func<Task> action, TimeSpan delay)
        {
            int attempts = tryCount;

            while (true)
            {
                try
                {
                    await action();
                    return;
                }
                catch
                {
                    if (--attempts == 0) throw;

                    await Task.Delay(delay);
                }
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    internal class CarRepository : ICarRepository
    {
        private readonly AppContext context;

        public CarRepository(AppContext context)
        {
            this.context = context;
        }

        public async Task CreateAsync(Car car, CancellationToken cancellationToken = default)
        {
            await context.Cars.AddAsync(car, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Car?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Cars.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task UpdateAsync(Car car, CancellationToken cancellationToken = default)
        {
            context.Cars.Update(car);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Car>> GetAvailableCarsAsync(CancellationToken cancellationToken = default)
        {
            return await context.Cars.Where(c => c.IsAvailable).ToListAsync(cancellationToken);
        }
    }
}

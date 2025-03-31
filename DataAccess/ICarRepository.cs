using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccess
{
    public interface ICarRepository
    {
        Task CreateAsync(Car car, CancellationToken cancellationToken = default);
        Task<Car?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(Car car, CancellationToken cancellationToken = default);
        Task<IEnumerable<Car>> GetAvailableCarsAsync(CancellationToken cancellationToken = default); 
    }
}

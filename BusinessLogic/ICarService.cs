using DataAccess;

namespace BusinessLogic
{
    public interface ICarService
    {
        Task CreateAsync(string text, decimal rentalPricePerDay, CancellationToken cancellationToken = default);
        Task<string> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateAsync(int id, string newText, CancellationToken cancellationToken = default);
        Task<IEnumerable<Car>> GetAvailableCarsAsync(CancellationToken cancellationToken = default); // Просмотр доступных автомобилей
        Task RentCarAsync(int id, DateTime rentalStartDate, DateTime rentalEndDate, CancellationToken cancellationToken = default); // Аренда автомобиля
        Task ReturnCarAsync(int id, CancellationToken cancellationToken = default); // Возврат автомобиля
        Task<decimal> CalculateRentalPriceAsync(int id, DateTime rentalStartDate, DateTime rentalEndDate, CancellationToken cancellationToken = default); // Расчет стоимости аренды
    }
}

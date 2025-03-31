using DataAccess;

namespace BusinessLogic
{
    internal class CarService : ICarService
    {
        private readonly ICarRepository carRepository;

        public CarService(ICarRepository carRepository)
        {
            this.carRepository = carRepository;
        }

        public async Task CreateAsync(string text, decimal rentalPricePerDay, CancellationToken cancellationToken = default)
        {
            var car = new Car
            {
                Text = text,
                RentalPricePerDay = rentalPricePerDay,
                IsAvailable = true
            };

            await carRepository.CreateAsync(car, cancellationToken);
        }

        public async Task<string> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var car = await carRepository.GetByIdAsync(id, cancellationToken);
            if (car == null)
                throw new Exception("Car not found");
            return car.Text;
        }

        public async Task UpdateAsync(int id, string newText, CancellationToken cancellationToken = default)
        {
            var car = await carRepository.GetByIdAsync(id, cancellationToken);
            if (car == null)
                throw new Exception("Car not found");
            car.Text = newText;
            await carRepository.UpdateAsync(car, cancellationToken);
        }

        public async Task<IEnumerable<Car>> GetAvailableCarsAsync(CancellationToken cancellationToken = default)
        {
            var cars = await carRepository.GetAvailableCarsAsync(cancellationToken);
            return cars;
        }

        public async Task RentCarAsync(int id, DateTime rentalStartDate, DateTime rentalEndDate, CancellationToken cancellationToken = default)
        {
            var car = await carRepository.GetByIdAsync(id, cancellationToken);
            if (car == null)
                throw new Exception("Car not found");

            if (!car.IsAvailable)
                throw new Exception("Car is not available for rent");

            car.IsAvailable = false;
            car.RentalStartDate = rentalStartDate;
            car.RentalEndDate = rentalEndDate;

            await carRepository.UpdateAsync(car, cancellationToken);
        }

        public async Task ReturnCarAsync(int id, CancellationToken cancellationToken = default)
        {
            var car = await carRepository.GetByIdAsync(id, cancellationToken);
            if (car == null)
                throw new Exception("Car not found");

            car.IsAvailable = true;
            car.RentalStartDate = null;
            car.RentalEndDate = null;

            await carRepository.UpdateAsync(car, cancellationToken);
        }

        public async Task<decimal> CalculateRentalPriceAsync(int id, DateTime rentalStartDate, DateTime rentalEndDate, CancellationToken cancellationToken = default)
        {
            var car = await carRepository.GetByIdAsync(id, cancellationToken);
            if (car == null)
                throw new Exception("Car not found");

            if (rentalStartDate > rentalEndDate)
                throw new Exception("Rental end date cannot be earlier than start date");

            var rentalDays = (rentalEndDate - rentalStartDate).Days;
            if (rentalDays <= 0)
                throw new Exception("Rental period must be at least one day");

            return car.RentalPricePerDay * rentalDays;
        }
    }
}

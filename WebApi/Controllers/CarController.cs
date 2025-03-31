using BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarController : ControllerBase
    {
        private readonly ICarService carService;

        public CarController(ICarService carService)
        {
            this.carService = carService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCarRequest request)
        {
            await carService.CreateAsync(request.Text, request.RentalPricePerDay);
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCarAsync([FromRoute] int id)
        {
            var result = await carService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableCarsAsync()
        {
            var cars = await carService.GetAvailableCarsAsync();
            return Ok(cars);
        }

        [HttpPut("{id:int}/rent")]
        public async Task<IActionResult> RentCarAsync([FromRoute] int id, [FromBody] RentCarRequest request)
        {
            await carService.RentCarAsync(id, request.RentalStartDate, request.RentalEndDate);
            return NoContent();
        }

        [HttpPut("{id:int}/return")]
        public async Task<IActionResult> ReturnCarAsync([FromRoute] int id)
        {
            await carService.ReturnCarAsync(id);
            return NoContent();
        }

        [HttpGet("{id:int}/calculate-price")]
        public async Task<IActionResult> CalculateRentalPriceAsync([FromRoute] int id, [FromQuery] DateTime rentalStartDate, [FromQuery] DateTime rentalEndDate)
        {
            var price = await carService.CalculateRentalPriceAsync(id, rentalStartDate, rentalEndDate);
            return Ok(price);
        }
    }
}

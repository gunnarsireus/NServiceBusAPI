using Client.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using Shared.Models;
using Shared.Requests;
using Shared.Responses;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Client.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarApiController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMessageSession _messageSession;

        public CarApiController(SignInManager<ApplicationUser> signInManager, IMessageSession messageSession)
        {
            _signInManager = signInManager;
            _messageSession = messageSession;
        }

        [HttpGet("getallcars")]
        [SwaggerOperation(Summary = "Get all cars", Description = "Get all cars")]
        [SwaggerResponse(200, "Respons was successfully generated")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> GetAllCars()
        {
            var response = await _messageSession.Request<GetCarsResponse>(new GetCarsRequest());
            return Ok(response.Cars); // Use Ok() to return JSON response
        }

        [HttpPost("updateonline")]
        [SwaggerOperation(Summary = "Update property online", Description = "Update property online")]
        [SwaggerResponse(200, "Update was successfull")]
        [SwaggerResponse(500, "An unexpected error occurred")]
        public async Task<IActionResult> UpdateOnline([FromBody] Car car)
        {
            if (!ModelState.IsValid) return BadRequest(new { success = false }); // Use BadRequest() for invalid model state

            var getCarResponse = await _messageSession.Request<GetCarResponse>(new GetCarRequest(car.Id));
            var oldCar = getCarResponse.Car;
            if (oldCar != null)
            {
                oldCar.Online = car.Online;
                await _messageSession.Request<UpdateCarResponse>(new UpdateCarRequest(oldCar));
            }

            return Ok(new { success = true }); // Use Ok() to return success response
        }
    }
}

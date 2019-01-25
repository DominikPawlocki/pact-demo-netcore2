using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pact.Provider.Api.Models;
using Pact.Provider.Api.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;

namespace Pact.Provider.Api.Controllers
{
    [Produces("application/json")]
    [Route("provider/api/cars")]
    [ApiController]
    public class CarsController : Controller
    {
        private readonly INhtsaService _service;

        public CarsController(INhtsaService service)
        {
            _service = service;
        }

        [HttpGet("manufacturers/first")]
        public async Task<ActionResult<NhtsaManufacturersResponce>> GetFirst100()
        {
            var result = await _service.GetFirst100Manufacturers();
            return Ok(result);
        }

        [HttpGet("manufacturers/{manufacturer}/details")]
        public async Task<ActionResult<NhtsaManufacturerDetailsResponce>> GetManufacturerDetails(string manufacturer)
        {
            var result = await _service.GetManufacturerDetails(manufacturer);
            if (result.Results == null || !result.Results.Any())
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("manufacturers/random20")]
        public async Task<ActionResult<NhtsaManufacturersResponce>> GetRandom20()
        {
            var result = await _service.GetRandom20Manufacturers();
            Request.HttpContext.Response.Headers.Add("Korean", "Ssanyong in header");
            return Ok(result);
        }

        [HttpGet("manufacturers/{manufacturer}/models/{year}")]
        public async Task<ActionResult<NhtsaCarModelResponce>> GetModels(string manufacturer, int year)
        {
            var result = await _service.GetModels(manufacturer, year);
            return Ok(result);
        }

        [HttpGet("vin/{vin}")]
        public async Task<ActionResult<NhtsaVINdecoderResponce>> DecodeVin(string vin)
        {
            var result = await _service.DecodeVin(vin);
            if (result.Results != null ||
               !string.IsNullOrWhiteSpace(result.Results.FirstOrDefault().ErrorCode))
            {
                result.Count = 1;
            }
            return Ok(result);
        }

        [HttpPost("vin")]
        public ActionResult<NhtsaVINResponce> UpsertVin(NhtsaVINRequest car)
        {
            Request.HttpContext.Response.Headers.Add("Location", "Ssanyong in header");
            return Created("/provider/api/cars", new NhtsaVINResponce
            {
                Id = 15421,
                Vin = car.Vin,
                Message = "Car added/modified correctly."
            });
        }
    }
}
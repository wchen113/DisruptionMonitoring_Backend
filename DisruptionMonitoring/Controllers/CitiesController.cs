using DisruptionMonitoring.Data;
using DisruptionMonitoring.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisruptionMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly DataContext _context;

        public CitiesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<City>>> GetAllCities()
        {
            var cities = await _context.City.Where(c => !c.IsDeleted).ToListAsync();
            return Ok(cities);
        }

        [HttpGet("top1000")]
        public async Task<ActionResult<List<City>>> GetTop1000Cities()
        {
            var cities = await _context.City
                .Where(c => !c.IsDeleted)
                .Take(1000)
                .ToListAsync();

            return Ok(cities);
        }

    }
}

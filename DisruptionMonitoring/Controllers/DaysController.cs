using DisruptionMonitoring.Data;
using DisruptionMonitoring.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisruptionMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DaysController : ControllerBase
    {
        private readonly DataContext _context;

        public DaysController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Days>>> GetAllDays()
        {
            var days = await _context.Days.Where(d => !d.IsDeleted).ToListAsync();
            return Ok(days);
        }
    }
}

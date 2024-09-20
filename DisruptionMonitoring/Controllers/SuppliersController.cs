using DisruptionMonitoring.Data;
using DisruptionMonitoring.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DisruptionMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly DataContext _context;

        public SuppliersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Suppliers>>> GetAllArticles()
        {
            var suppliers = await _context.Suppliers.Where(s => !s.IsDeleted).ToListAsync();
            return Ok(suppliers);
        }
    }
}

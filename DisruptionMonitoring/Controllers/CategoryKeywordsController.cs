using DisruptionMonitoring.Data;
using DisruptionMonitoring.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisruptionMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryKeywordsController : ControllerBase
    {
        private readonly DataContext _context;

        public CategoryKeywordsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryKeywords>>> GetAllCategories()
        {
            var categories = await _context.CategoryKeywords.Where(a => !a.IsDeleted).ToListAsync();
            return Ok(categories);
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchArticlesByCategory(string category)
        {
            var keywords = await _context.CategoryKeywords
                .Where(ck => ck.Category.ToLower() == category.ToLower() && !ck.IsDeleted)
                .Select(ck => ck.Keyword)
                .FirstOrDefaultAsync();

            if (keywords == null)
            {
                return NotFound(); 
            }

            // Forward request to GoogleSearchController with the selected keywords as query
            return Redirect($"/api/GoogleSearch?q={keywords}");
        }

        [HttpPost]
        public async Task<ActionResult<CategoryKeywords>> AddCategoryKeyword(CategoryKeywords categoryKeyword)
        {
            if (categoryKeyword == null)
            {
                return BadRequest("CategoryKeyword cannot be null.");
            }

            // Validate if the keyword already exists for the given category
            var existingKeyword = await _context.CategoryKeywords
                .Where(ck => ck.Category.ToLower() == categoryKeyword.Category.ToLower() &&
                              ck.Keyword.ToLower() == categoryKeyword.Keyword.ToLower() &&
                              !ck.IsDeleted)
                .FirstOrDefaultAsync();

            if (existingKeyword != null)
            {
                return Conflict("This keyword already exists for the specified category.");
            }

            _context.CategoryKeywords.Add(categoryKeyword);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAllCategories), new { id = categoryKeyword.Id }, categoryKeyword);
        }

    }
}

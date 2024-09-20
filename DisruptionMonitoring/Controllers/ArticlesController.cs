using DisruptionMonitoring.Data;
using DisruptionMonitoring.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisruptionMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly DataContext _context;

        public ArticlesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Articles>>> GetAllArticles()
        {
            var articles = await _context.Articles.Where(a => !a.IsDeleted).ToListAsync();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Articles>> GetArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null || article.IsDeleted)
            {
                return NotFound();
            }

            return Ok(article);
        }

        [HttpGet("articles")]
        public IActionResult GetArticles(DateTime? fromDate, DateTime? toDate)
        {
            var articles = _context.Articles.AsQueryable();

            if (fromDate.HasValue && toDate.HasValue)
            {
                DateOnly fromDateOnly = DateOnly.FromDateTime(fromDate.Value);
                DateOnly toDateOnly = DateOnly.FromDateTime(toDate.Value);

                articles = articles.Where(a => a.PublishedDate >= fromDateOnly && a.PublishedDate <= toDateOnly);
            }

            return Ok(new { values = articles.ToList() });
        }

        [HttpPost]
        public async Task<ActionResult<List<Articles>>> AddArticle([FromBody] Articles article)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return Ok(await _context.Articles.Where(a => !a.IsDeleted).ToListAsync());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateArticle(int id, Articles updatedArticle)
        {
            if (id != updatedArticle.Id)
            {
                return BadRequest();
            }

            var article = await _context.Articles.FindAsync(id);

            if (article == null || article.IsDeleted)
            {
                return NotFound();
            }

            article.Title = updatedArticle.Title;
            article.Text = updatedArticle.Text;
            article.Location = updatedArticle.Location;
            article.Lat = updatedArticle.Lat;
            article.Lng = updatedArticle.Lng;
            article.DisruptionType = updatedArticle.DisruptionType;
            article.Severity = updatedArticle.Severity;
            article.SourceName = updatedArticle.SourceName;
            article.PublishedDate = updatedArticle.PublishedDate;
            article.Url = updatedArticle.Url;
            article.ImageUrl = updatedArticle.ImageUrl;
            article.Radius = updatedArticle.Radius;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null || article.IsDeleted)
            {
                return NotFound();
            }

            article.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("nearby")]
        public async Task<ActionResult<List<Articles>>> GetNearbyArticles([FromQuery] decimal lat, [FromQuery] decimal lng, [FromQuery] double radius = 5000)
        {
            if (radius <= 0)
            {
                return BadRequest("Radius must be greater than zero.");
            }

            // Retrieve all articles that are not deleted
            var articles = await _context.Articles
                .Where(a => !a.IsDeleted)
                .ToListAsync();

            // Filter articles by distance in-memory
            var nearbyArticles = articles
                .Where(a => GetDistance((double)lat, (double)lng, (double)a.Lat, (double)a.Lng) <= radius)
                .ToList();

            return Ok(nearbyArticles);
        }


        // Calculate distance between two coordinates
        private double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            const double EarthRadiusKm = 6371.0; // Radius of the Earth in kilometers

            double dLat = ToRadians(lat2 - lat1);
            double dLng = ToRadians(lng2 - lng1);
            double lat1Rad = ToRadians(lat1);
            double lat2Rad = ToRadians(lat2);

            // Haversine formula
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // Distance in meters
            double distance = EarthRadiusKm * c * 1000;
            return distance;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }


    }
}

using DisruptionMonitoring.Data;
using DisruptionMonitoring.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisruptionMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<UserProfilesController> _logger;

        public UserProfilesController(DataContext context, ILogger<UserProfilesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/UserProfiles
        [HttpGet]
        public async Task<ActionResult<List<UserProfiles>>> GetAllProfiles()
        {
            try
            {
                var userProfiles = await _context.UserProfiles
                    .Where(p => !p.IsDeleted)  // Exclude deleted profiles
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(userProfiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all profiles.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/UserProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfiles>> GetProfile(int id)
        {
            try
            {
                var userProfile = await _context.UserProfiles
                    .Include(p => p.CategoryKeywords)
                    .Include(p => p.Suppliers)
                    .Include(p => p.Locations)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (userProfile == null)
                    return NotFound(new { message = "Profile not found" });

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving profile with ID {ProfileId}.", id);
                return StatusCode(500, "Internal server error");
            }
        }


        // GET: api/UserProfiles/GetProfileByEmail?email=johndoe@example.com
        [HttpGet("GetProfileByEmail")]
        public async Task<ActionResult<UserProfiles>> GetProfileByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email cannot be empty");

            try
            {
                var userProfile = await _context.UserProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Email == email && !p.IsDeleted);

                if (userProfile == null)
                    return NotFound(new { message = "User profile not found." });

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving profile with email {Email}.", email);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpsertProfile([FromBody] UserProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var profile = request.Profile;

                if (profile.Id > 0)
                {
                    // Update existing profile
                    var existingProfile = await _context.UserProfiles
                        .FirstOrDefaultAsync(p => p.Id == profile.Id && !p.IsDeleted);

                    if (existingProfile == null)
                        return NotFound(new { message = "Profile not found." });

                    // Update properties
                    existingProfile.Name = profile.Name;
                    existingProfile.Email = profile.Email;

                    // Remove old relationships before adding new ones
                    _context.UserProfilesCategoryKeywords.RemoveRange(
                        _context.UserProfilesCategoryKeywords.Where(uk => uk.UserProfileId == profile.Id));
                    _context.UserProfilesSupplier.RemoveRange(
                        _context.UserProfilesSupplier.Where(us => us.UserProfileId == profile.Id));
                    _context.UserProfilesLocation.RemoveRange(
                        _context.UserProfilesLocation.Where(ul => ul.UserProfileId == profile.Id));

                    // Update the existing profile
                    _context.UserProfiles.Update(existingProfile);
                    await _context.SaveChangesAsync();

                    // Add new relationships
                    await AddCategoryKeywordsAndSuppliers(request, profile.Id);

                    return Ok(existingProfile);
                }
                else
                {
                    // Create new profile
                    var emailExists = await _context.UserProfiles
                        .AnyAsync(p => p.Email == profile.Email);

                    if (emailExists)
                        return Conflict(new { message = "Email is already in use." });

                    _context.UserProfiles.Add(profile);
                    await _context.SaveChangesAsync(); // Save changes to generate profile ID

                    // Add new relationships using the generated ID
                    await AddCategoryKeywordsAndSuppliers(request, profile.Id);

                    return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing profile.");
                return StatusCode(500, "Internal server error");
            }
        }


        // PUT: api/UserProfiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UserProfiles profile)
        {
            if (id != profile.Id)
                return BadRequest(new { message = "Profile ID mismatch" });

            try
            {
                var dbProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (dbProfile == null)
                    return NotFound(new { message = "Profile not found" });

                // Update properties
                dbProfile.Name = profile.Name;
                dbProfile.Email = profile.Email;

                // Save changes
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating profile with ID {ProfileId}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/UserProfiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

                if (profile == null)
                    return NotFound(new { message = "Profile not found" });

                // Mark as deleted
                profile.IsDeleted = true;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting profile with ID {ProfileId}.", id);
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task AddCategoryKeywordsAndSuppliers(UserProfileRequest request, int profileId)
        {
            // Lists to track invalid IDs
            var invalidCategoryKeywordIds = new List<int>();
            var invalidSupplierIds = new List<int>();
            var invalidLocationIds = new List<int>();

            // Retrieve and attach CategoryKeywords
            if (request.CategoryKeywordIds != null)
            {
                foreach (var categoryId in request.CategoryKeywordIds)
                {
                    var categoryKeyword = await _context.CategoryKeywords.FindAsync(categoryId);

                    if (categoryKeyword != null)
                    {
                        _context.UserProfilesCategoryKeywords.Add(new UserProfilesCategoryKeywords
                        {
                            UserProfileId = profileId,
                            CategoryKeywordId = categoryKeyword.Id,
                            CategoryName = categoryKeyword.Category,
                            IsDeleted = false
                        });
                    }
                    else
                    {
                        invalidCategoryKeywordIds.Add(categoryId);
                        _logger.LogWarning("CategoryKeyword with ID {CategoryId} does not exist.", categoryId);
                    }
                }
            }

            // Retrieve and attach Suppliers
            if (request.SupplierIds != null)
            {
                foreach (var supplierId in request.SupplierIds)
                {
                    var supplier = await _context.Suppliers.FindAsync(supplierId);

                    if (supplier != null)
                    {
                        _context.UserProfilesSupplier.Add(new UserProfilesSupplier
                        {
                            UserProfileId = profileId,
                            SupplierId = supplier.Id,
                            IsDeleted = false
                        });
                    }
                    else
                    {
                        invalidSupplierIds.Add(supplierId);
                        _logger.LogWarning("Supplier with ID {SupplierId} does not exist.", supplierId);
                    }
                }
            }

            // Retrieve and attach Locations
            if (request.LocationIds != null)
            {
                foreach (var locationId in request.LocationIds)
                {
                    var location = await _context.City.FindAsync(locationId);

                    if (location != null)
                    {
                        _context.UserProfilesLocation.Add(new UserProfilesLocation
                        {
                            UserProfileId = profileId,
                            LocationId = location.Id,
                            IsDeleted = false
                        });
                    }
                    else
                    {
                        invalidLocationIds.Add(locationId);
                        _logger.LogWarning("Location with ID {LocationId} does not exist.", locationId);
                    }
                }
            }

            // Log warnings if there are any invalid IDs
            if (invalidCategoryKeywordIds.Any() || invalidSupplierIds.Any() || invalidLocationIds.Any())
            {
                _logger.LogWarning("Failed to add some relationships. Invalid CategoryKeyword IDs: {InvalidCategoryKeywordIds}. Invalid Supplier IDs: {InvalidSupplierIds}. Invalid Location IDs: {InvalidLocationIds}.",
                                   invalidCategoryKeywordIds, invalidSupplierIds, invalidLocationIds);
            }

            await _context.SaveChangesAsync();
        }

    }
}

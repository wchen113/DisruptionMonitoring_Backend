using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DisruptionMonitoring.Entities
{
    public class UserProfiles
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Email { get; set; }

        public ICollection<UserProfilesCategoryKeywords> CategoryKeywords { get; set; } = new List<UserProfilesCategoryKeywords>();
        public ICollection<UserProfilesSupplier> Suppliers { get; set; } = new List<UserProfilesSupplier>();
        public ICollection<UserProfilesLocation> Locations { get; set; } = new List<UserProfilesLocation>();

        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;
    }

    public class UserProfilesCategoryKeywords
    {
        public int Id { get; set; }

        public int UserProfileId { get; set; }
        public UserProfiles UserProfile { get; set; }

        public int CategoryKeywordId { get; set; }
        public string? CategoryName { get; set; }
        public CategoryKeywords CategoryKeyword { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class UserProfilesSupplier
    {
        public int Id { get; set; }

        public int UserProfileId { get; set; }
        public UserProfiles UserProfile { get; set; }

        public int SupplierId { get; set; }
        public Suppliers Supplier { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class UserProfilesLocation
    {
        public int Id { get; set; }

        public int UserProfileId { get; set; }
        public UserProfiles UserProfile { get; set; }

        public int LocationId { get; set; }
        public City Location { get; set; }

        public bool IsDeleted { get; set; }
    }

}

using DisruptionMonitoring.Entities;
using Microsoft.EntityFrameworkCore;

namespace DisruptionMonitoring.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Articles> Articles { get; set; }
        public DbSet<CategoryKeywords> CategoryKeywords { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<Days> Days { get; set; }
        public DbSet<UserProfiles> UserProfiles { get; set; }
        public DbSet<UserProfilesCategoryKeywords> UserProfilesCategoryKeywords { get; set; }
        public DbSet<UserProfilesSupplier> UserProfilesSupplier { get; set; }
        public DbSet<UserProfilesLocation> UserProfilesLocation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserProfilesCategoryKeywords>()
                .HasKey(uc => new { uc.UserProfileId, uc.CategoryKeywordId });

            modelBuilder.Entity<UserProfilesCategoryKeywords>()
                .HasOne(uc => uc.UserProfile)
                .WithMany(up => up.CategoryKeywords)
                .HasForeignKey(uc => uc.UserProfileId);

            modelBuilder.Entity<UserProfilesCategoryKeywords>()
                .HasOne(uc => uc.CategoryKeyword)
                .WithMany()
                .HasForeignKey(uc => uc.CategoryKeywordId);

            modelBuilder.Entity<UserProfilesSupplier>()
                .HasOne(us => us.UserProfile)
                .WithMany(up => up.Suppliers)
                .HasForeignKey(us => us.UserProfileId);

            modelBuilder.Entity<UserProfilesSupplier>()
                .HasOne(us => us.Supplier)
                .WithMany()
                .HasForeignKey(us => us.SupplierId);

            modelBuilder.Entity<UserProfilesLocation>()
                .HasOne(ul => ul.UserProfile)
                .WithMany(up => up.Locations)
                .HasForeignKey(ul => ul.UserProfileId);

            modelBuilder.Entity<UserProfilesLocation>()
                .HasOne(ul => ul.Location)
                .WithMany()
                .HasForeignKey(ul => ul.LocationId);

            modelBuilder.Entity<UserProfilesCategoryKeywords>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }

    }
}

using CMCS_PROG6212_POE.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS_PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ClaimModel> Claims { get; set; } // Ensure this is here
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ensure ClaimModel's HourlyRate has precision
            modelBuilder.Entity<ClaimModel>()
                .Property(c => c.HourlyRate)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}

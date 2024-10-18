using CMCS_PROG6212_POE.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS_PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ClaimModel> Claims { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClaimModel>()
                .Property(c => c.HourlyRate)
                .HasPrecision(18, 2);  // Set precision for decimal values

            base.OnModelCreating(modelBuilder);
        }
    }
}

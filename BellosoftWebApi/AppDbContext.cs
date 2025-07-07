using Microsoft.EntityFrameworkCore;

namespace BellosoftWebApi
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Add relations here

            base.OnModelCreating(modelBuilder);
        }
    }
}

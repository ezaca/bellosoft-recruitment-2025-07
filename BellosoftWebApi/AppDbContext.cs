using BellosoftWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BellosoftWebApi
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Deck> Decks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(BuildUserTable);
            modelBuilder.Entity<Deck>(BuildDeckTable);
            base.OnModelCreating(modelBuilder);
        }

        private void BuildUserTable(EntityTypeBuilder<User> entity)
        {
            entity.HasKey(user => user.Id);

            entity.Property(user => user.Email)
                  .IsUnicode()
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(user => user.Password)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.HasOne(user => user.SelectedDeck)
                  .WithMany()
                  .HasForeignKey(user => user.SelectedDeckId)
                  .OnDelete(DeleteBehavior.Restrict);
        }

        private void BuildDeckTable(EntityTypeBuilder<Deck> entity)
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.ExternalName)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(d => d.RemainingCards)
                  .IsRequired();

            entity.HasOne(d => d.User)
                  .WithMany()
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

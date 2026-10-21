using Microsoft.EntityFrameworkCore;
using TicketApp.Domain;

namespace TicketApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {

  }

  public DbSet<Account> Accounts => Set<Account>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Account>(entity =>
    {
      entity.ToTable("accounts");
      entity.HasKey(a => a.Id);
      entity.Property(a => a.Id).HasColumnName("id");
      entity.Property(a => a.Name).HasColumnName("name").IsRequired();
      entity.Property(a => a.Surname).HasColumnName("surname").IsRequired();
      entity.Property(a => a.Email).HasColumnName("email").IsRequired();
      entity.Property(a => a.PasswordHash).HasColumnName("password_hash").IsRequired();
      entity.Property(a => a.CreatedAtUtc).HasColumnName("created_at");
      entity.HasIndex(a => a.Email).IsUnique();
    });
  }
}
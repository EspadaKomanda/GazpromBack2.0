using Microsoft.EntityFrameworkCore;
using UserService.Database.Models;
namespace UserService.Database;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<RegistrationCode> RegistrationCodes { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role {Id = 1, Name = "user", Protected = true},
            new Role {Id = 2, Name = "admin", Protected = true}
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

}
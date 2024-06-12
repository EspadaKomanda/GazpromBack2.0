using Microsoft.EntityFrameworkCore;
using DialogService.Database.Models;
namespace DialogService.Database;

public class ApplicationContext : DbContext
{
    public DbSet<Dialog> Dialogs { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

}

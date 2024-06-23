using ImageAgregationService.Models;
using ImageAgregationService.Singletones;
using Microsoft.EntityFrameworkCore;

namespace ImageAgregationService.Database
{
    public class ApplicationContext : DbContext
    {
        private readonly ConfigReader _configReader;
        public DbSet<ImageModel> Images { get; set; } = null!;
        public DbSet<TemplateModel> Templates {get;set;} = null!;
        public DbSet<MarkModel> Marks {get;set;} = null!;
        public DbSet<KeyWordModel> KeyWords {get;set;} = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options, ConfigReader configReader) : base(options)
        {
            _configReader = configReader;
            Database.EnsureCreated();
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }
    }
}
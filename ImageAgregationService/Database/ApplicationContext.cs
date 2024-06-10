using ImageAgregationService.Models;
using ImageAgregationService.Singletones;
using Microsoft.EntityFrameworkCore;

namespace ImageAgregationService.Database
{
    public class ApplicationContext : DbContext
    {
        private readonly ConfigReader _configReader;
        public DbSet<ImageModel> Images { get; set; }
        public DbSet<TemplateModel> Templates {get;set;}
        public DbSet<MarkModel> Marks {get;set;}

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
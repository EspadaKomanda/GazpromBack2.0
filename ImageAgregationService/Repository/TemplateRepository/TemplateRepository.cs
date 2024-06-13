using ImageAgregationService.Database;
using ImageAgregationService.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageAgregationService.Repository
{
    public class TemplateRepository(ApplicationContext db) : ITemplateRepository
    {
        private readonly ApplicationContext _db = db;
        public async Task<bool> CreateTemplate(TemplateModel obj)
        {
            await _db.Templates.AddAsync(obj);
            return await Save();
        }
    
        public async Task<bool> DeleteTemplate(TemplateModel obj)
        {
            _db.Templates.Remove(obj);
            return await Save();
        }

        public async Task GenerateTemplates(List<string> templates)
        {
            foreach (var template in templates)
            {
                if(!await DoesTemplateExist(template))
                {
                    await CreateTemplate(new TemplateModel{Name = template, DefaultPrompt="testprompt"});
                }
            }
        }

        public async Task<TemplateModel?> GetTemplateByName(string name)
        {
            return await _db.Templates.FirstOrDefaultAsync(x => x.Name == name);
        }

        public IQueryable<TemplateModel> GetTemplates()
        {
            return _db.Templates;
        }

        public async Task<bool> DoesTemplateExist(string name)
        {
            return await _db.Templates.AnyAsync(x => x.Name == name);
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() >= 0;
        }
    
        public async Task<bool> UpdateTemplate(TemplateModel obj)
        {
            _db.Templates.Update(obj);
            return await Save();
        }

    
    }
    
}
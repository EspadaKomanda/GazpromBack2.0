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
            if(await _db.Templates.AnyAsync(x=>x.Name == "pictures"))
            {
                await CreateTemplate(new TemplateModel{Name = "pictures", DefaultPrompt="Generate picture"});
            }
            if(await _db.Templates.AnyAsync(x=>x.Name == "zoompictures"))
            {
                await CreateTemplate(new TemplateModel{Name = "zoompictures", DefaultPrompt="Generate zoom picture"});
            }
            if(await _db.Templates.AnyAsync(x=>x.Name == "backgrounds"))
            {
                await CreateTemplate(new TemplateModel{Name = "backgrounds", DefaultPrompt="Generate background"});
            }
            if(await _db.Templates.AnyAsync(x=>x.Name == "avatars"))
            {
                await CreateTemplate(new TemplateModel{Name = "avatars", DefaultPrompt="Generate avatar"});
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
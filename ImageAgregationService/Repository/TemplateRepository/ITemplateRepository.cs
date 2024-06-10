namespace ImageAgregationService.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ImageAgregationService.Models;

    public interface ITemplateRepository
    {
        public Task GenerateTemplates(List<string> templates);
        public IQueryable<TemplateModel> GetTemplates();
        public Task<bool> CreateTemplate(TemplateModel obj);
        public Task<bool> UpdateTemplate(TemplateModel obj);
        public Task<bool> DeleteTemplate(TemplateModel obj);
        public Task<TemplateModel> GetTemplateByName(string name);
        public Task<bool> IsTemplateExist(string name);
        public Task<bool> Save();
    }
}
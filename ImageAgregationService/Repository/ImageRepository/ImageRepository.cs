using ImageAgregationService.Database;
using ImageAgregationService.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageAgregationService.Repository.ImageRepository
{
    public class ImageRepository(ApplicationContext db) : IImageRepository
    {
        
        private readonly ApplicationContext _db = db;
        public async Task<bool> CreateImage(ImageModel obj)
        {
            await _db.Images.AddAsync(obj);
            return await Save();
        }

        public async Task<bool> DeleteImage(ImageModel obj)
        {
            _db.Images.Remove(obj);
            return await Save();
        }

        public async Task<bool> DeleteImagesByTemplate(Guid id)
        {
            _db.Images.RemoveRange(_db.Images.Where(x => x.TemplateId == id));
            return await Save();
        }

        public async Task<ImageModel?> GetImageById(Guid id)
        {
            return _db.Images.Include(x=>x.Mark).Include(x=>x.Template).Include(x=>x.KeyWords).First(x=>x.Id == id);
        }

        public IQueryable<ImageModel> GetImages()
        {
            return _db.Images.Include(x=>x.Mark).Include(x=>x.Template).Include(x=>x.KeyWords);
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() >= 0;
        }

        public Task<bool> UpdateImage(ImageModel obj)
        {
            _db.Images.Update(obj);
            return Save();
        }
    }
}
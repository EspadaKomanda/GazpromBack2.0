using ImageAgregationService.Models;

namespace ImageAgregationService.Repository.ImageRepository
{
    public interface IImageRepository
    {
        public IQueryable<ImageModel> GetImages();
        public Task<bool> CreateImage(ImageModel obj);
        public Task<bool> UpdateImage(ImageModel obj);
        public Task<bool> DeleteImage(ImageModel obj);
        public Task<ImageModel?> GetImageById(Guid id);
        public Task<bool> DeleteImagesByTemplate(Guid id);
        public Task<bool> Save();
    }
}
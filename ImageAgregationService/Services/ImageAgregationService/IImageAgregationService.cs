using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;

namespace ImageAgregationService.Services.ImageAgregationService
{
    public interface IImageAgregationService
    {
        Task<ImageDto> GetImage(string key,GenerateImageKafkaRequest generateImageRequest);
        
    }
}
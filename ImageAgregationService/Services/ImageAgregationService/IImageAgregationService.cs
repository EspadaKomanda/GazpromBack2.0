using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;

namespace ImageAgregationService.Services.ImageAgregationService
{
    public interface IImageAgregationService
    {
        Task<ImageDto> GetImage(string key,GenerateImageKafkaRequest generateImageRequest);
        Task<List<ImageDto>> GetImages(GetImagesKafkaRequest getImagesRequest);
        Task<string> GetLikedImages();
        Task<string> GetSpecificImages(GetImagesKafkaRequest getImagesRequest);
        Task<int> GetPagesCount();
        Task<List<ImageDto>> GetImagesPage(GetPage page);
        List<KeyWordDTO> GetUniqueKeyWords();
        List<ImageDto> GetImagesByKeywords(GetImagesByKeywords getImagesByKeywords);
    }
}
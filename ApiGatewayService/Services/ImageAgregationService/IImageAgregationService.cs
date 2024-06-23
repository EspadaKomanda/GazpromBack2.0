using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;

namespace ApiGatewayService.Services.ImageAgregationService
{
    public interface IImageAgregationService
    {
        Task<ImageDto> GenerateImage(GenerateImageKafkaRequest generateImageKafkaRequest);
        Task<List<ImageDto>> GetImageDto(GetImagesKafkaRequest getImagesRequest);
        Task<string> GetLikedImages();
        Task<string> GetSpecificImages(GetImagesKafkaRequest getImagesRequest);
        Task<int> GetPagesCount();
        Task<List<ImageDto>> GetImagesPage(GetPage page);
        Task<List<KeyWordDTO>> GetUniqueKeyWords();
        Task<List<ImageDto>> GetImagesByKeywords(GetImagesByKeywords getImagesByKeywords);
    
    }
}
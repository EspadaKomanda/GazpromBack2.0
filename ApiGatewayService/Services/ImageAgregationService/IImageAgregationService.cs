using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;

namespace ApiGatewayService.Services.ImageAgregationService
{
    public interface IImageAgregationService
    {
        Task<ImageDto> GenerateImage(GenerateImageKafkaRequest generateImageKafkaRequest);
        Task<List<ImageDto>> GetImageDto(GetImagesKafkaRequest getImagesRequest);
    }
}
using ImageAgregationService.Models;
using ImageAgregationService.Models.DTO;
using Imagegenerator;
using ImageProcessor;

namespace ImageAgregationService.Services
{
    public interface IS3Service
    {
        Task ConfigureBuckets();
        Task<bool> UploadImageToS3Bucket(ImageResponse generateImageResponse, string template);
        Task<bool> DeleteImageFromS3Bucket(string fileName, string bucketName);
        Task<ImageModel> GetImageFromS3Bucket(string fileName, string bucketName);
    }
}
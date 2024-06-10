using ImageAgregationService.Models;
using ImageAgregationService.Models.DTO;
using Imagegenerator;
using Imagetextadder;

namespace ImageAgregationService.Services
{
    public interface IS3Service
    {
        Task ConfigureBuckets();
        Task<bool> UploadImageToS3Bucket(GenerateImageResponse generateImageResponse);
        Task<bool> DeleteImageFromS3Bucket(string fileName, string bucketName);
        Task<ImageModel> GetImageFromS3Bucket(string fileName, string bucketName);
    }
}
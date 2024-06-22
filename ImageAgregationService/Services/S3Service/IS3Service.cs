using ImageAgregationService.Models;
using ImageAgregationService.Models.DTO;
using Imagegenerator;
using ImageProcessor;

namespace ImageAgregationService.Services
{
    public interface IS3Service
    {
        Task ConfigureBuckets();
        Task<bool> UploadImageToS3Bucket(ImageResponse generateImageResponse, string template, string imageName);
        Task<bool> DeleteImageFromS3Bucket(string fileName, string bucketName);
        Task<ImageModel> GetImageFromS3Bucket(string fileName, string bucketName);
        Task<bool> UploadArchieveToS3Bucket(ArchieveModel archieveModel);
        Task<string> GetArchieveFromS3Bucket();
        Task<bool> CheckIfBucketExists(string bucketName);
        Task<bool> DeleteBucket(string bucketName);
        Task<bool> CreateBucket(string bucketName);
    }
}
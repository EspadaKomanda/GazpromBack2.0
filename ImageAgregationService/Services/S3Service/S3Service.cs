using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using ImageAgregationService.Exceptions.ConfigExceptions;
using ImageAgregationService.Exceptions.S3ServiceExceptions;
using ImageAgregationService.Models;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Singletones;
using Imagegenerator;
using ImageProcessor;

namespace ImageAgregationService.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<S3Service> _logger;
        private readonly ConfigReader _configReader;
        public S3Service(IAmazonS3 s3Client, ILogger<S3Service> logger, ConfigReader configReader)
        {
            _s3Client = s3Client;
            _logger = logger;
            _configReader = configReader;
            
        }
        public async Task ConfigureBuckets()
        {
            try
            {
                List<string> bucketNames = await _configReader.GetBuckets();
                foreach (var bucketName in bucketNames)
                {
                    if(!await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client,bucketName))
                    {
                        await _s3Client.PutBucketAsync(bucketName);
                    }   
                }
                
                foreach(var bucketName in bucketNames)
                {
                    if(!await CheckIfBucketExists(bucketName))
                    {
                        _logger.LogError("Failed to configure S3 buckets, storage unavailable!");
                        throw new StorageUnavailibleException("Failed to configure S3 buckets, storage unavailable!");
                    }   
                }
                _logger.LogInformation("S3 buckets configured!");
            }
            catch (Exception ex)
            {
                if (ex is GetConfigException)
                {
                    _logger.LogError(ex, "Failed to read buckets from config file!");
                    throw new ConfigureBucketsException("Failed to read buckets from config file!", ex);
                }
                _logger.LogError(ex, "Failed to configure S3 buckets!");
                throw new ConfigureBucketsException("Failed to configure S3 buckets!", ex);
            }
        }
        private async Task<bool> CheckIfBucketExists(string bucketName)
        {
            return await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        }
        public async Task<bool> DeleteBucket(string bucketName)
        {
            try
            {
                DeleteBucketResponse response = await _s3Client.DeleteBucketAsync(bucketName);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Bucket {bucketName} deleted!");
                    return true;
                }
                if (response.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError($"Bucket {bucketName} not found!");
                    throw new BucketNotFoundException($"Bucket {bucketName} not found!");
                }
                if (response.HttpStatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    _logger.LogError($"Failed to delete bucket {bucketName}!");
                    throw new DeleteBucketException($"Failed to delete bucket {bucketName}!");
                }
                _logger.LogError($"Failed to delete bucket {bucketName}, unhandled exception!");
                throw new DeleteBucketException($"Failed to delete bucket {bucketName}, unhandled exception!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete bucket {bucketName}!");
                throw new DeleteBucketException($"Failed to delete bucket {bucketName}!", ex);
            }
        }
        public async Task<bool> DeleteImageFromS3Bucket(string fileName, string bucketName)
        {
            try
            {
                DeleteObjectResponse response = await _s3Client.DeleteObjectAsync(bucketName, fileName);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Image {fileName} deleted from S3 bucket {bucketName}!");
                    return true;
                }
                if (response.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError($"Image {fileName} not found in S3 bucket {bucketName}!");
                    throw new ImageNotFoundException($"Image {fileName} not found in S3 bucket {bucketName}!");
                }
                if (response.HttpStatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    _logger.LogError($"Failed to delete image {fileName} from S3 bucket {bucketName}, storage unavailible!");
                    throw new DeleteImageException($"Failed to delete image {fileName} from S3 bucket {bucketName}, storage unavailible!");
                }
                _logger.LogError($"Failed to delete image {fileName} from S3 bucket {bucketName}, unhandled exception!" + response.ToString());
                throw new DeleteImageException($"Failed to delete image {fileName} from S3 bucket {bucketName}!, unhandled exception!" + response.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete image from S3 bucket!");
                throw new DeleteImageException("Failed to delete image from S3 bucket!", ex);
            }
        }

        public async Task<ImageModel> GetImageFromS3Bucket(string fileName, string bucketName)
        {
            try
            {
                GetObjectMetadataResponse metadataResponse = await _s3Client.GetObjectMetadataAsync(bucketName, fileName);
                if(metadataResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    var response =  _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest(){ BucketName = bucketName, Key = fileName, Expires = DateTime.Now.AddMinutes(10), Protocol = Protocol.HTTP});
                    _logger.LogInformation($"Uri for image {fileName} aquired from S3 bucket {bucketName}!");
                    return new ImageModel
                    {
                        Name = fileName,
                        Url = response,
                        Mark = new MarkModel(),
                        Template = new TemplateModel()
                    };
                }
                if(metadataResponse.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError($"Image {fileName} not found in S3 bucket {bucketName}!");
                    throw new ImageNotFoundException($"Image {fileName} not found in S3 bucket {bucketName}!");
                }
                if(metadataResponse.HttpStatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    _logger.LogError($"Failed to get image {fileName} from S3 bucket {bucketName}, storage unavailible!");
                    throw new GetImageException($"Failed to get image {fileName} from S3 bucket {bucketName}, storage unavailible!");
                }
                _logger.LogError($"Failed to get image {fileName} from S3 bucket {bucketName}, unhandled exception!" + metadataResponse.ToString());
                throw new GetImageException($"Failed to get image {fileName} from S3 bucket {bucketName}!, unhandled exception!" + metadataResponse.ToString());
                                    
            }      
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get image from S3 bucket!");
                throw new GetImageException("Failed to get image from S3 bucket!", ex);
            }
        }
        public async Task<bool> UploadImageToS3Bucket(ImageResponse generateImageResponse, string template, string imageName)
        {
            try
            {
                PutObjectResponse response = await _s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = template,
                    Key = imageName,
                    InputStream = new MemoryStream(generateImageResponse.ImageBytes.ToByteArray())
                });
                if(response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Image {generateImageResponse.ImageBytes} uploaded to S3 bucket {template}!");
                    return true;
                }
                if(response.HttpStatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    _logger.LogError($"Failed to upload image {imageName} to S3 bucket {template}, storage unavailable!");
                    throw new StorageUnavailibleException("Failed to upload image to S3 bucket, storage unavailable!");
                }
                _logger.LogError($"Failed to upload image {imageName} to S3 bucket {template}, unhandled exception!" + response.ToString());
                throw new UploadImageException($"Failed to upload image {imageName} to S3 bucket {template}, unhandled exception!" + response.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload image to S3 bucket!");
                throw new UploadImageException("Failed to upload image to S3 bucket!", ex);
            }
        }
    }
}
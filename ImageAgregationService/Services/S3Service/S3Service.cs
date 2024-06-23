using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using ImageAgregationService.Exceptions.ConfigExceptions;
using ImageAgregationService.Exceptions.S3ServiceExceptions;
using ImageAgregationService.Models;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Repository;
using ImageAgregationService.Singletones;
using Imagegenerator;
using ImageProcessor;

namespace ImageAgregationService.Services
{ 
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly ILogger<S3Service> _logger;
        private readonly ITemplateRepository _templateRepository;
        public S3Service(IAmazonS3 s3Client, ILogger<S3Service> logger, ConfigReader configReader, ITemplateRepository templateRepository)
        {
            _s3Client = s3Client;
            _logger = logger;
            _templateRepository = templateRepository;
        }
        public async Task ConfigureBuckets()
        {
            try
            {
                var buckets =  _templateRepository.GetTemplates();
                
                foreach (var bucket in buckets)
                {
                    if(!await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client,bucket.Guid.ToString()))
                    {
                        await _s3Client.PutBucketAsync(bucket.Guid.ToString());
                    }   
                }
                
                foreach(var bucket in buckets)
                {
                    if(!await CheckIfBucketExists(bucket.Guid.ToString()))
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
        public async Task<bool> CreateBucket(string bucketName)
        {
            try
            {
                if(!await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client,bucketName))
                {
                    await _s3Client.PutBucketAsync(bucketName);
                    return true;
                }   
                _logger.LogInformation($"Bucket {bucketName} already exists!");
                throw new BucketAlreadyExistsException($"Bucket {bucketName} already exists!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create bucket!");
                throw new ConfigureBucketsException("Failed to create bucket!", ex);
            }
        }
        public async Task<string> GetArchieveFromS3Bucket()
        {
            try
            {
                var response =  _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest(){ BucketName = "archieves", Key = "LikedImages", Expires = DateTime.Now.AddYears(10), Protocol = Protocol.HTTP});
                _logger.LogInformation($"Archieve URL: {response}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get archieve from S3 bucket!");
                throw new GetImageException("Failed to get archieve from S3 bucket!", ex);
            }
        }
        public async Task<string> GetArchieveFromS3Bucket(string archieveName)
        {
            try
            {
                var response =  _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest(){ BucketName = "archieves", Key = archieveName, Expires = DateTime.Now.AddYears(10), Protocol = Protocol.HTTP});
                _logger.LogInformation($"Archieve URL: {response}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get archieve from S3 bucket!");
                throw new GetImageException("Failed to get archieve from S3 bucket!", ex);
            }
        }
        public async Task<bool> UploadArchieveToS3Bucket(ArchieveModel archieveModel)
        {
            try
            {
                Console.WriteLine(archieveModel.archieveData.Length);
                DeleteObjectResponse deleteObjectResponse = await _s3Client.DeleteObjectAsync("archieves", archieveModel.archieveName);
                
                PutObjectResponse response = await _s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = "archieves",
                    Key = archieveModel.archieveName,
                    InputStream = new MemoryStream(archieveModel.archieveData),
                    ContentType = archieveModel.archieveType
                });

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation($"Archieve {archieveModel.archieveName} uploaded to S3 bucket!");
                    return true;
                }
                if (response.HttpStatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    _logger.LogError($"Failed to upload archieve {archieveModel.archieveName} to S3 bucket!");
                    throw new UploadImageException("Failed to upload archieve to S3 bucket!");
                }
                if (response.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogError($"Bucket archieves not found!");
                    throw new BucketNotFoundException("Bucket archieves not found!");
                }
                _logger.LogError($"Failed to upload archieve {archieveModel.archieveName} to S3 bucket!");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload archieve to S3 bucket!");
                throw new UploadImageException("Failed to upload archieve to S3 bucket!", ex);
            }
        }
       
        public async Task<bool> CheckIfBucketExists(string bucketName)
        {
            return await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        }
        public async Task<bool> DeleteBucket(string bucketName)
        {
            try
            {
                DeleteBucketResponse response = await _s3Client.DeleteBucketAsync(bucketName);
                _logger.LogInformation(response.HttpStatusCode.ToString()+ response.ResponseMetadata.ToString());
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
                    var response =  _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest(){ BucketName = bucketName, Key = fileName, Expires = DateTime.Now.AddYears(10), Protocol = Protocol.HTTP});
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
                    InputStream = new MemoryStream(generateImageResponse.ImageBytes.ToByteArray()),
                    ContentType = "image/png"
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
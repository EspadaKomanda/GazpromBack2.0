using System.Text;
using Confluent.Kafka;
using ImageAgregationService.Exceptions.GenerateImageExceptions;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using KafkaTestLib.Kafka;
using KafkaTestLib.KafkaException;
using Newtonsoft.Json;

namespace ApiGatewayService.Services.ImageAgregationService
{
    public class ImageAgregationService : IImageAgregationService
    {
        private readonly ILogger<ImageAgregationService> _logger;
        private readonly KafkaService _kafkaService;
        public ImageAgregationService(ILogger<ImageAgregationService> logger, KafkaService kafkaService)
        {
            _logger = logger;
            _kafkaService = kafkaService;
        }
        public async Task<string> GetLikedImages()
        {
            try
            {
                Guid currentId = Guid.NewGuid();
                if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("IMAGEREQ_TOPIC") ?? "",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key =currentId.ToString(),
                    Value = "",
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("getLikedImages")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var imageDto = await _kafkaService.Consume<string>(Environment.GetEnvironmentVariable("IMAGERESP_TOPIC") ?? "", currentId, "getLikedImages");
                    _logger.LogInformation("Get liked images successfully");
                    return imageDto;
                }

                _logger.LogError("Error getting liked images");
                throw new GetImagesException("Error getting liked images");
            }
            catch (Exception ex)
            {
                if (ex is not MyKafkaException)
                {
                    _logger.LogError(ex,"Error getting liked images");
                    throw new GetImagesException("Error getting liked images",ex);
                }
                _logger.LogError(ex,"Unhandled error");
                throw;
            }
        }
        public async Task<ImageDto> GenerateImage(GenerateImageKafkaRequest generateImageKafkaRequest)
        {
            try
            {
                Guid currentId = Guid.NewGuid();
                if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("IMAGEREQ_TOPIC") ?? "",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key =currentId.ToString(),
                    Value = JsonConvert.SerializeObject(generateImageKafkaRequest),
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("generateImage")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var imageDto = await _kafkaService.Consume<ImageDto>(Environment.GetEnvironmentVariable("IMAGERESP_TOPIC") ?? "", currentId, "generateImage");
                    _logger.LogInformation("Image generated successfully, Text: {Text}, Template: {Template}", generateImageKafkaRequest.Text , generateImageKafkaRequest.TemplateName);
                    return imageDto;
                }

                _logger.LogError("Error sending image, Text: {Text}, Template: {Template}", generateImageKafkaRequest.Text , generateImageKafkaRequest.TemplateName);
                throw new GenerateImageException("Error sending image, Text: "+generateImageKafkaRequest.Text+" Template: "+generateImageKafkaRequest.TemplateName);
            }
            catch (Exception e)
            {
                if (e is not MyKafkaException)
                {
                    _logger.LogError(e,"Error generating image, Text: {Text}, Template: {Template}", generateImageKafkaRequest.Text , generateImageKafkaRequest.TemplateName);
                    throw new GenerateImageException("Error generating image, Text: "+generateImageKafkaRequest.Text+" Template: "+generateImageKafkaRequest.TemplateName,e);
                }
                _logger.LogError(e,"Unhandled error");
                throw;
            }
        }

        public async Task<List<ImageDto>> GetImageDto(GetImagesKafkaRequest getImagesRequest)
        {
            try
            {
                if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("IMAGEREQ_TOPIC") ?? "",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key = Guid.NewGuid().ToString(),
                    Value = JsonConvert.SerializeObject(getImagesRequest),
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("getImages")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var imageDto = await _kafkaService.Consume<List<ImageDto>>(Environment.GetEnvironmentVariable("IMAGERESP_TOPIC") ?? "", Guid.NewGuid(), "getImages");
                    _logger.LogInformation("Images fetched successfully, Ids: {ids}", JsonConvert.SerializeObject(getImagesRequest));
                    return imageDto;
                }
                _logger.LogError("Error fetching images, Ids: {ids}", JsonConvert.SerializeObject(getImagesRequest));   
                throw new GetImagesException("Error fetching images, Ids: "+JsonConvert.SerializeObject(getImagesRequest));
            }
            catch (Exception e)
            {
                if (e is not MyKafkaException)
                {
                    _logger.LogError(e,"Error fetching images, Ids: {ids}", JsonConvert.SerializeObject(getImagesRequest));
                    throw new GetImagesException("Error fetching images, Ids: "+JsonConvert.SerializeObject(getImagesRequest),e);
                }
                _logger.LogError(e,"Unhandled error");
                throw;
            }
        }
    }
}
using ImageAgregationService.Exceptions.GenerateImageExceptions;
using ImageAgregationService.Exceptions.TemplateExceptions;
using ImageAgregationService.Models;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels;
using ImageAgregationService.Repository;
using ImageAgregationService.Repository.ImageRepository;
using ImageAgregationService.Singletones.Communicators;
using Imagegenerator;
using Imagetextadder;
using Imageverifier;
using KafkaTestLib.Kafka;
using KafkaTestLib.KafkaException;
using KafkaTestLib.Models;
using Newtonsoft.Json;

namespace ImageAgregationService.Services.ImageAgregationService
{
    public class ImageAgregationService : IImageAgregationService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ILogger<ImageAgregationService> _logger;
        private readonly KafkaProducer _kafkaProducer;
        private readonly IS3Service _s3Service;
        private readonly ImageGenerationCommunicator _imageGenerationCommunicator;
        private readonly ImageVerifierCommunicator _imageVerifierCommunicator;
        private readonly ImageTextAdderCommunicator _imageTextAdderCommunicator;
        public ImageAgregationService(IImageRepository imageRepository, ITemplateRepository templateRepository, ILogger<ImageAgregationService> logger, IS3Service s3Service, ImageGenerationCommunicator imageGenerationCommunicator, ImageVerifierCommunicator imageVerifierCommunicator, ImageTextAdderCommunicator imageTextAdderCommunicator, KafkaProducer kafkaProducer)
        {
            _imageRepository = imageRepository;
            _templateRepository = templateRepository;
            _logger = logger;
            _s3Service = s3Service;
            _imageGenerationCommunicator = imageGenerationCommunicator;
            _imageVerifierCommunicator = imageVerifierCommunicator;
            _imageTextAdderCommunicator = imageTextAdderCommunicator;
            _kafkaProducer = kafkaProducer;
        }
        
        public async Task GetImage(string key,GenerateImageKafkaRequest generateImageRequest)
        {  
            try
            {
                var template = await _templateRepository.IsTemplateExist(generateImageRequest.TemplateName);
                if(!template)
                {
                    _logger.LogError("Template not found");
                    throw new TemplateNotFoundException("Template not found");
                }
                string prompt =await GenerateValidPrompt(generateImageRequest.TemplateName, generateImageRequest.Text);
                GenerateImageResponse image = await SendGenerateImageRequest(prompt); 
                AddTextToImageResponse imageWithText = await SendAddTextToImageRequest(await SendVerifyImageRequest(image), generateImageRequest.Text);
                image.ImageByteArray = imageWithText.ImageByteArray;
                if(await _s3Service.UploadImageToS3Bucket(image))
                {
                    ImageModel imageModel = await _s3Service.GetImageFromS3Bucket(image.ImageName,generateImageRequest.TemplateName);
                    await _imageRepository.CreateImage(imageModel);
                    _logger.LogInformation("Saved image model: " + image.ImageName);
                    ProduceResponseModel responseStatus = await _kafkaProducer.Produce("generatedImages", new Confluent.Kafka.Message<string, string>(){ Key = "addimageresponse"+key, Value = JsonConvert.SerializeObject(imageModel)});
                    if(responseStatus.Success)
                    {
                        _logger.LogInformation("Sended image: " + image.ImageName);
                        return;
                    }
                    _logger.LogError("Failed to send image: " + image.ImageName + " with error: " + responseStatus.ErrorMessage);

                }
            }
            catch (Exception e)
            {
                if(!(e is MyKafkaException))
                {
                    _logger.LogError(e,"Error generating image");
                    ProduceResponseModel responseStatus = await _kafkaProducer.Produce("generatedImages", new Confluent.Kafka.Message<string, string>(){ Key = "addimageresponse_error"+key, Value = JsonConvert.SerializeObject(e)});
                    throw new GenerateImageException("Error generating image",e);
                }
                _logger.LogError(e,"Kafka error");
                throw new GenerateImageException("Error sending image response",e);
            }
        }
        private async Task<GenerateImageResponse> SendGenerateImageRequest(string prompt)
        {
            GenerateImageResponse image = await _imageGenerationCommunicator.GenerateImage(prompt);
            if(image.Error != "")
            {
                _logger.LogError("Failed to generate image" + image.Error);
                throw new GenerateImageException("Failed to generate image");
            }
            _logger.LogInformation("Generated image: " + image.ImageName + " with prompt: " + prompt);
            return image;
        }
        private async Task<VerifyImageResponse> SendVerifyImageRequest(GenerateImageResponse image)
        {
            VerifyImageResponse verifyImageResponse = await _imageVerifierCommunicator.VerifyImage(image);
            if(verifyImageResponse.Error != "")
            {
                _logger.LogError("Failed to verify image" + verifyImageResponse.Error);
                throw new VerifyImageException("Failed to verify image");
            }
            return verifyImageResponse;
        }
        private async Task<AddTextToImageResponse> SendAddTextToImageRequest(VerifyImageResponse verifiedImage, string text)
        {
            AddTextToImageResponse addTextToImageResponse = await _imageTextAdderCommunicator.AddText(verifiedImage, text);
            if(addTextToImageResponse.Error != "")
            {
                _logger.LogError("Failed to add text to image" + addTextToImageResponse.Error);
                throw new AddTextToImageException("Failed to add text to image");
            }
            return addTextToImageResponse;
        }
        private async Task<string> GenerateValidPrompt(string templateName, string text)
        {
            try
            {
                TemplateModel template = await _templateRepository.GetTemplateByName(templateName);
                if(template == null)
                {
                    _logger.LogError("Template not found");
                    throw new TemplateNotFoundException("Template not found");
                }
                string prompt = template.DefaultPrompt + text;
                _logger.LogInformation("Generated prompt: " + prompt);
                return prompt;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to generate prompt");
                throw new GeneratePromptException("Failed to generate prompt", ex);
            }
        }
    }
}
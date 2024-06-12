using ImageAgregationService.Exceptions.GenerateImageExceptions;
using ImageAgregationService.Exceptions.S3ServiceExceptions;
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
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ImageAgregationService.Services.ImageAgregationService
{
    public class ImageAgregationService : IImageAgregationService
    {
        private readonly IImageRepository _imageRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ILogger<ImageAgregationService> _logger;
        private readonly IS3Service _s3Service;
        private readonly ImageGenerationCommunicator _imageGenerationCommunicator;
        private readonly ImageVerifierCommunicator _imageVerifierCommunicator;
        private readonly ImageTextAdderCommunicator _imageTextAdderCommunicator;
        private readonly IDistributedCache _cache;
        public ImageAgregationService(IImageRepository imageRepository, 
        ITemplateRepository templateRepository, 
        ILogger<ImageAgregationService> logger,
        IS3Service s3Service, 
        ImageGenerationCommunicator imageGenerationCommunicator, 
        ImageVerifierCommunicator imageVerifierCommunicator, 
        ImageTextAdderCommunicator imageTextAdderCommunicator, 
        IDistributedCache cache)
        {
            _imageRepository = imageRepository;
            _templateRepository = templateRepository;
            _logger = logger;
            _s3Service = s3Service;
            _imageGenerationCommunicator = imageGenerationCommunicator;
            _imageVerifierCommunicator = imageVerifierCommunicator;
            _imageTextAdderCommunicator = imageTextAdderCommunicator;
            _cache = cache;
        }
        
        public async Task<ImageDto> GetImage(string key,GenerateImageKafkaRequest generateImageRequest)
        {  
            try
            {
                var cachedImage = await _cache.GetStringAsync(generateImageRequest.TemplateName+generateImageRequest.Text);
                if(cachedImage==null)
                {
                    var IsTemplateExist = await _templateRepository.IsTemplateExist(generateImageRequest.TemplateName);
                    if(!IsTemplateExist)
                    {
                        _logger.LogError("Template not found");
                        throw new TemplateNotFoundException("Template not found");
                    }
                    string prompt =await GenerateValidPrompt(generateImageRequest.TemplateName, generateImageRequest.Text);
                    GenerateImageResponse image = await SendGenerateImageRequest(prompt); 
                    AddTextToImageResponse imageWithText = await SendAddTextToImageRequest(await SendVerifyImageRequest(image), generateImageRequest.Text);
                    image.ImageByteArray = imageWithText.ImageByteArray;
                    if(!await _s3Service.UploadImageToS3Bucket(image))
                    {  
                        _logger.LogError("Error uploading image");
                        throw new UploadImageException("Error uploading image");
                    }
                    ImageModel imageModel = await _s3Service.GetImageFromS3Bucket(image.ImageName,generateImageRequest.TemplateName);
                    TemplateModel currentTemplate = await _templateRepository.GetTemplateByName(generateImageRequest.TemplateName);
                    imageModel.Template = currentTemplate;
                    imageModel.Mark = new MarkModel(){ Name = "none"};
                    await _imageRepository.CreateImage(imageModel);
                    _logger.LogInformation("Saved image model: " + image.ImageName);
                    ImageDto filalImage = new ImageDto()
                    {
                        Name = imageModel.Name,
                        Url = imageModel.Url,
                        mark = new MarkDto()
                        {
                            Name = imageModel.Mark.Name
                        },
                        template = new TemplateDto()
                        {
                            Name = imageModel.Template.Name
                        }
                    };
                    await _cache.SetStringAsync(generateImageRequest.TemplateName+generateImageRequest.Text, JsonConvert.SerializeObject(filalImage));
                    return filalImage;
                }
                _logger.LogInformation("Found cached image" + generateImageRequest.TemplateName+generateImageRequest.Text);
                return JsonConvert.DeserializeObject<ImageDto>(cachedImage);
                
            }
            catch (Exception e)
            {
               
                _logger.LogError(e,"Error generating image");
                throw new GenerateImageException("Error generating image",e);
                
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

        public async Task<List<ImageDto>> GetImages(GetImagesKafkaRequest getImagesRequest)
        {
            try 
            {
                List<ImageDto> imageDtos = new List<ImageDto>();
                foreach (var imageName in getImagesRequest.Ids)
                {
                    ImageModel image = await _imageRepository.GetImageById(imageName);
                    if(image == null)
                    {
                        _logger.LogError("Image not found! Image name: " + imageName);
                        throw new ImageNotFoundException("Image not found! Image name: " + imageName);
                    }
                    imageDtos.Add(new ImageDto()
                    {
                        Id = image.Id,
                        Name = image.Name,
                        Url = image.Url,
                        mark = new MarkDto()
                        {
                            Name = image.Mark.Name
                        },
                        template = new TemplateDto()
                        {
                            Name = image.Template.Name
                        }
                    });
                }
                return imageDtos;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get images!");
                throw new GetImagesException("Failed to get images!", ex);
            }
        }
    }
}
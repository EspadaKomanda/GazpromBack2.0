using ImageAgregationService.Exceptions.MarkExceptions;
using ImageAgregationService.Exceptions.S3ServiceExceptions;
using ImageAgregationService.Models;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels.Mark;
using ImageAgregationService.Repository.ImageRepository;
using ImageAgregationService.Repository.MarkRepository;
using Newtonsoft.Json;

namespace ImageAgregationService.Services.MarkService
{
    public class MarkService : IMarkService
    {
        private readonly IMarkRepository _markRepository;
        private readonly ILogger<MarkService> _logger;
        private readonly IImageRepository _imageRepository;
        public MarkService(IMarkRepository markRepository, ILogger<MarkService> logger, IImageRepository imageRepository)
        {
            _markRepository = markRepository;
            _logger = logger;
            _imageRepository = imageRepository;
        } 
        
        public async Task<MarkDto> UpdateMark(UpdateMarkKafkaRequest updateMark)
        {
            try
            {
                _logger.LogInformation("Update mark: {Name}", JsonConvert.SerializeObject(updateMark));
                ImageModel image = await _imageRepository.GetImageById(updateMark.ImageId);
                if(image==null)
                {
                    _logger.LogError("Image not found!");
                    throw new ImageNotFoundException("Image not found!");
                }
                image.Mark.Name = updateMark.MarkDto.Name;
                await _markRepository.UpdateMark(image.Mark);
                return new MarkDto(){
                    Name = image.Mark.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add mark!");
                throw new AddMarkException("Failed to add mark!", ex);
            }
        }
    }
}
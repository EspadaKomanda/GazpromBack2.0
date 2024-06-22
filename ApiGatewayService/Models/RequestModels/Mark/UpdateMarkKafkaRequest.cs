using ImageAgregationService.Models.DTO;

namespace ImageAgregationService.Models.RequestModels.Mark
{
    public class UpdateMarkKafkaRequest
    {
        public MarkDto MarkDto{ get; set; }
        public Guid ImageId { get; set; }
    }
}
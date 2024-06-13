namespace ImageAgregationService.Models.RequestModels
{
    public class GetImagesKafkaRequest
    {
        public List<Guid> Ids { get; set; } = null!;
    }
}
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels.Mark;

namespace ImageAgregationService.Services.MarkService
{
    public interface IMarkService
    {
        Task<MarkDto> UpdateMark(UpdateMarkKafkaRequest updateMark);
    }
}
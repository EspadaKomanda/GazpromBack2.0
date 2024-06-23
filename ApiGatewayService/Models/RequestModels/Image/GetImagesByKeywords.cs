using ImageAgregationService.Models.DTO;

namespace ImageAgregationService.Models.RequestModels
{
    public class GetImagesByKeywords
    {
        public List<KeyWordDTO> KeyWords {get;set;}
    }
}
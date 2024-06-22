using System.Text;
using Confluent.Kafka;
using ImageAgregationService.Exceptions.MarkExceptions;
using ImageAgregationService.Models.DTO;
using ImageAgregationService.Models.RequestModels.Mark;
using KafkaTestLib.Kafka;
using KafkaTestLib.KafkaException;
using Newtonsoft.Json;

namespace ImageAgregationService.Services.MarkService
{
    public class MarkService : IMarkService
    {
        private readonly ILogger<MarkService> _logger;
        private readonly KafkaService _kafkaService;
        public MarkService(ILogger<MarkService> logger, KafkaService kafkaService)
        {
            _logger = logger;
            _kafkaService = kafkaService;
        }

        public async Task<MarkDto> UpdateMark(UpdateMarkKafkaRequest updateMark)
        {
            try
            {
                Guid messageId = Guid.NewGuid();
                if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("IMAGEREQ_TOPIC") ?? "",
                new Confluent.Kafka.Message<string, string>(){ 
                    Key = messageId.ToString(),
                    Value = JsonConvert.SerializeObject(updateMark),
                    Headers = new Headers(){
                        new Header("method",Encoding.UTF8.GetBytes("updateMark")),
                        new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                    }
                }))
                {
                    var markDto = await _kafkaService.Consume<MarkDto>( Environment.GetEnvironmentVariable("IMAGERESP_TOPIC") ?? "", messageId, "updateMark");
                    _logger.LogInformation("Mark updated successfully, Mark: {Mark}", JsonConvert.SerializeObject(updateMark));
                    return markDto;
                }
                _logger.LogError("Error sending mark, Mark: {Mark}", JsonConvert.SerializeObject(updateMark));
                throw new UpdateMarkException("Error sending mark, Mark: "+JsonConvert.SerializeObject(updateMark));
            }
            catch (Exception e)
            {
                if (e is not MyKafkaException)
                {
                    _logger.LogError(e,"Error updating mark, Mark: {Mark}", JsonConvert.SerializeObject(updateMark));
                    throw new UpdateMarkException("Error updating mark, Mark: "+JsonConvert.SerializeObject(updateMark),e);
                }
                _logger.LogError(e,"Unhandled error");
                throw;
            }
        }
    }
}
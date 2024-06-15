using System.Text;
using ApiGatewayService.Exceptions.DialogExceptions;
using BackGazprom.Models.Generic.Responses;
using Confluent.Kafka;
using DialogService.Database.Models;
using DialogService.Models.Requests;
using KafkaTestLib.Kafka;
using KafkaTestLib.KafkaException;
using Newtonsoft.Json;

namespace DialogService.Services.DialogsService;

public class DialogService : IDialogsService
{
    private readonly ILogger<DialogService> _logger;
    private readonly KafkaService _kafkaService;
    public DialogService( ILogger<DialogService> logger, KafkaService kafkaService)
    {
        _logger = logger;
        _kafkaService = kafkaService;
    }
    public async Task<bool> ClearDialog(ClearDialogRequest clearDialogRequest)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce("dialogRequestsTopic",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(clearDialogRequest),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("clearDialog")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var dialog = await _kafkaService.Consume<MessageResponse>("dialogResponseTopic", messageId, "clearDialog");
                _logger.LogInformation("Dialog cleared, Dialog: {Dialog}", JsonConvert.SerializeObject(clearDialogRequest));
                return !dialog.Message.Contains("Error");
            }
            _logger.LogError("Error sending dialog data, Dialog: {Dialog}", JsonConvert.SerializeObject(clearDialogRequest));
            throw new ClearDialogException("Error sending dialog, Dialog: "+JsonConvert.SerializeObject(clearDialogRequest));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error clearing dialog, Dialog: {Dialog}", JsonConvert.SerializeObject(clearDialogRequest));
                throw new ClearDialogException("Error clearing dialog, Dialog: "+JsonConvert.SerializeObject(clearDialogRequest),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }   
    }

    public Task<Dialog> CreateDialog(CreateDialogRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteDialog(DeleteDialogRequest deleteDialogRequest)
    {
        throw new NotImplementedException();
    }

    public Task<List<Message>?> GetDialogMessages(GetDialogMessagesRequest dialogId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Dialog>?> GetDialogsByOwnerId(GetDialogsByIdRequest getDialogsByIdRequest)
    {
        throw new NotImplementedException();
    }
}

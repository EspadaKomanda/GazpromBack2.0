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
            if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("DIALOGREQ_TOPIC") ?? "",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(clearDialogRequest),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("clearDialog")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var dialog = await _kafkaService.Consume<MessageResponse>(Environment.GetEnvironmentVariable("DIALOGRESP_TOPIC") ?? "", messageId, "clearDialog");
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

    public async Task<Dialog> CreateDialog(CreateDialogRequest request)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("DIALOGREQ_TOPIC") ?? "",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(request),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("createDialog")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var dialog = await _kafkaService.Consume<Dialog>(Environment.GetEnvironmentVariable("DIALOGRESP_TOPIC") ?? "", messageId, "createDialog");
                _logger.LogInformation("Dialog created, Dialog: {Dialog}", JsonConvert.SerializeObject(request));
                return dialog;
            }
            _logger.LogError("Error sending dialog data, Dialog: {Dialog}", JsonConvert.SerializeObject(request));
            throw new CreateDialogException("Error sending dialog, Dialog: "+JsonConvert.SerializeObject(request));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error creating dialog, Dialog: {Dialog}", JsonConvert.SerializeObject(request));
                throw new CreateDialogException("Error creating dialog, Dialog: "+JsonConvert.SerializeObject(request),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }   
    }

    public async Task<bool> DeleteDialog(DeleteDialogRequest deleteDialogRequest)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("DIALOGREQ_TOPIC") ?? "",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(deleteDialogRequest),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("deleteDialog")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var dialog = await _kafkaService.Consume<MessageResponse>(Environment.GetEnvironmentVariable("DIALOGRESP_TOPIC") ?? "", messageId, "deleteDialog");
                _logger.LogInformation("Dialog deleted, Dialog: {Dialog}", JsonConvert.SerializeObject(deleteDialogRequest));
                return !dialog.Message.Contains("Error");
            }
            _logger.LogError("Error sending dialog data, Dialog: {Dialog}", JsonConvert.SerializeObject(deleteDialogRequest));
            throw new DeleteDialogException("Error sending dialog, Dialog: "+JsonConvert.SerializeObject(deleteDialogRequest));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error deleting dialog, Dialog: {Dialog}", JsonConvert.SerializeObject(deleteDialogRequest));
                throw new DeleteDialogException("Error deleting dialog, Dialog: "+JsonConvert.SerializeObject(deleteDialogRequest),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }   
    }

    public async Task<List<Message>?> GetDialogMessages(GetDialogMessagesRequest dialogId)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("DIALOGREQ_TOPIC") ?? "",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(dialogId),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("getDialogMessages")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var dialog = await _kafkaService.Consume<List<Message>>(Environment.GetEnvironmentVariable("DIALOGRESP_TOPIC") ?? "", messageId, "getDialogMessages");
                _logger.LogInformation("Dialog messages received, Dialog: {Dialog}", JsonConvert.SerializeObject(dialogId));
                return dialog;
            }
            _logger.LogError("Error sending dialog data, Dialog: {Dialog}", JsonConvert.SerializeObject(dialogId));
            throw new GetDialogMessagesException("Error sending dialog, Dialog: "+JsonConvert.SerializeObject(dialogId));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error getting dialog messages, Dialog: {Dialog}", JsonConvert.SerializeObject(dialogId));
                throw new GetDialogMessagesException("Error getting dialog messages, Dialog: "+JsonConvert.SerializeObject(dialogId),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }

    public async Task<List<Dialog>?> GetDialogsByOwnerId(GetDialogsByIdRequest getDialogsByIdRequest)
    {
        try
        {
            Guid messageId = Guid.NewGuid();
            if(await _kafkaService.Produce( Environment.GetEnvironmentVariable("DIALOGREQ_TOPIC") ?? "",
            new Confluent.Kafka.Message<string, string>(){ 
                Key = messageId.ToString(),
                Value = JsonConvert.SerializeObject(getDialogsByIdRequest),
                Headers = new Headers(){
                    new Header("method",Encoding.UTF8.GetBytes("getDialogsByOwnerId")),
                    new Header("sender",Encoding.UTF8.GetBytes("apiGatewayService"))
                }
            }))
            {
                var dialog = await _kafkaService.Consume<List<Dialog>>(Environment.GetEnvironmentVariable("DIALOGRESP_TOPIC") ?? "", messageId, "getDialogsByOwnerId");
                _logger.LogInformation("Dialogs received, Dialog: {Dialog}", JsonConvert.SerializeObject(getDialogsByIdRequest));
                return dialog;
            }
            _logger.LogError("Error sending dialog data, Dialog: {Dialog}", JsonConvert.SerializeObject(getDialogsByIdRequest));
            throw new GetDialogsByOwnerIdException("Error sending dialog, Dialog: "+JsonConvert.SerializeObject(getDialogsByIdRequest));
        }
        catch (Exception e)
        {
            if (e is not MyKafkaException)
            {
                _logger.LogError(e,"Error getting dialogs, Dialog: {Dialog}", JsonConvert.SerializeObject(getDialogsByIdRequest));
                throw new GetDialogsByOwnerIdException("Error getting dialogs, Dialog: "+JsonConvert.SerializeObject(getDialogsByIdRequest),e);
            }
            _logger.LogError(e,"Unhandled error");
            throw;
        }
    }
} 

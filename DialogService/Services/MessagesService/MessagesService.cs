using DialogService.Database.Models;
using DialogService.Enums;
using DialogService.Exceptions.Messages;
using DialogService.Models.Messages.Requests;
using DialogService.Repositories;

namespace DialogService.Services.MessagesService;

public class MessagesService(IMessageRepository messageRepo, IDialogRepository dialogRepo, ILogger<MessagesService> logger) : IMessagesService
{
    private readonly IMessageRepository _messageRepo = messageRepo;
    private readonly IDialogRepository _dialogRepo = dialogRepo;
    private readonly ILogger<MessagesService> _logger = logger;

    public async Task<bool> DeleteMessage(DeleteMessageRequest obj)
    {
        var message = await _messageRepo.GetMessageById(obj.MessageId);
        var dialog = await _dialogRepo.GetDialogById(obj.DialogId);
        try
        {
            if (dialog == null || dialog.OwnerId != obj.OwnerId || message == null || dialog.Id != message.DialogId)
            {
                throw new MessageNotFoundException();
            }
        }
        catch (MessageNotFoundException e)
        {
            _logger.LogError(e, "Message {Id} being attempted to be deleted by {OwnerId} for dialog {DialogId} is not owned or does not exist", obj.MessageId, obj.OwnerId, obj.DialogId);
            throw;
        }

        try
        {
            await _messageRepo.DeleteMessage(message);
            _logger.LogInformation("Message {Id} deleted", obj.MessageId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting message {Id}", obj.MessageId);
            throw;
        }
    }

   

    public async Task<Message> GetMessage(GetMessageRequest obj)
    {
        var message = await _messageRepo.GetMessageById(obj.MessageId);
        var dialog = await _dialogRepo.GetDialogById(obj.DialogId);

        try
        {
            if (dialog == null || dialog.OwnerId != obj.OwnerId || message == null || dialog.Id != message.DialogId)
            {
                throw new MessageNotFoundException();
            }
        }
        catch (MessageNotFoundException e)
        {
            _logger.LogError(e, "Message {Id} being attempted to be acquired by {OwnerId} for dialog {DialogId} is not owned or does not exist", obj.MessageId, obj.OwnerId, obj.DialogId);
            throw;
        }

        return message;
    }

    public Task<bool> RegenerateMessage(long ownerId, Sender accessor, long messageId)
    {
        throw new NotImplementedException();
    }

    public async Task<Message> SendMessage(SendMessageRequest obj)
    {
        var dialog = await _dialogRepo.GetDialogById(obj.DialogId);
        try
        {
            if (dialog == null || dialog.OwnerId != obj.OwnerId)
            {
                throw new DialogNotFoundException();
            }
        }
        catch (MessageNotFoundException e)
        {
            _logger.LogError(e, "Dialog {Id} not found for user {OwnerId}", obj.DialogId, obj.OwnerId);
            throw;
        }

        if (obj.Text == null)
        {
            _logger.LogError("Message being attempted to be sent by {OwnerId} for dialog {DialogId} has no text", obj.OwnerId, obj.DialogId);
            throw new MessageEmptyException();
        }

        var message = new Message
        {
            Sender = obj.Accessor == Sender.User ? Sender.User : obj.Sender,
            Style = obj.Accessor == Sender.User ? MessageStyle.Normal : obj.Style,
            ImageId = obj.Accessor == Sender.User ? null : obj.ImageId ?? null,
            Text = obj.Text,
            DialogId = obj.DialogId
        };

        try
        {
            await _messageRepo.AddMessage(message);
            _logger.LogInformation("Message {Id} sent", message.Id);
            return message;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while sending message {Id}", message.Id);
            throw;
        }
    }
}
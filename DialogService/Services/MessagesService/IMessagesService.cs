using DialogService.Database.Models;
using DialogService.Enums;
using DialogService.Models.Messages.Requests;

namespace DialogService.Services.MessagesService;

public interface IMessagesService
{
    public Task<Message> SendMessage(long ownerId, Sender accessor, SendMessageRequest obj);
    public Task<bool> RegenerateMessage(long ownerId, Sender accessor, long messageId);
    public Task<Message> GetMessage(long ownerId, Sender accessor, GetMessageRequest obj);
}
using DialogService.Database.Models;
using DialogService.Models.Messages.Requests;

namespace DialogService.Services.MessagesService;

//FIXME: Only operational for the user as of now
public interface IMessagesService
{
    public Task<Message> SendMessage(long ownerId, SendMessageRequest obj);
    public Task<Message> EditMessage(long ownerId, EditMessageRequest obj);
    public Task<bool> DeleteMessage(long ownerId, DeleteMessageRequest obj);
    public Task<bool> RegenerateMessage(long ownerId,long messageId);
    public Task<Message> GetMessage(long ownerId, GetMessageRequest obj);
}
using DialogService.Database.Models;
using DialogService.Enums;
using DialogService.Models.Messages.Requests;

namespace DialogService.Services.MessagesService;

public interface IMessagesService
{
    public Task<Message> SendMessage(SendMessageRequest obj);
    public Task<Message> GetMessage(GetMessageRequest obj);
    public Task<bool> DeleteMessage(DeleteMessageRequest obj);
}
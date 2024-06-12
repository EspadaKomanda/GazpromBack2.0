using DialogService.Database.Models;

namespace DialogService.Repositories;

public interface IMessageRepository
{
    Task<bool> AddMessage(Message obj);
    Task<bool> UpdateMessage(Message obj);
    Task<bool> DeleteMessage(Message obj);
    Task<bool> ClearAllInDialog(long dialogId);
    Task<Message?> GetMessageById(long id);
    Task<List<Message>> GetMessagesByDialogId(long dialogId);
    IQueryable<Message> GetMessages();
    Task<bool> Save();
}

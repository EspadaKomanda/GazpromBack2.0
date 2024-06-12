using DialogService.Database;
using DialogService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DialogService.Repositories;

public class MessageRepository(ApplicationContext db) : IMessageRepository
{
    private readonly ApplicationContext _db = db;

    public async Task<bool> AddMessage(Message obj)
    {
        _db.Messages.Add(obj);
        return await Save();
    }

    public async Task<bool> DeleteMessage(Message obj)
    {
        _db.Messages.Remove(obj);
        return await Save();
    }

    public async Task<Message?> GetMessageById(long id)
    {
        return await _db.Messages.FirstOrDefaultAsync(x => x.Id == id);
    }

    public IQueryable<Message> GetMessages()
    {
        return _db.Messages.AsQueryable();
    }

    public async Task<List<Message>> GetMessagesByDialogId(long dialogId)
    {
        return await _db.Messages.Where(x => x.DialogId == dialogId).ToListAsync();
    }

    public async Task<bool> Save()
    {
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateMessage(Message obj)
    {
        _db.Messages.Update(obj);
        return await Save();
    }
}

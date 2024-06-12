using DialogService.Database;
using DialogService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace DialogService.Repositories;

public class DialogRepository(ApplicationContext db) : IDialogRepository
{
    private readonly ApplicationContext _db = db;

    public async Task<bool> AddDialog(Dialog obj)
    {
        _db.Dialogs.Add(obj);
        return await Save();
    }

    public async Task<bool> DeleteDialog(Dialog obj)
    {
        _db.Dialogs.Remove(obj);
        return await Save();
    }

    public async Task<Dialog?> GetDialogById(long id)
    {
        return await _db.Dialogs.FirstOrDefaultAsync(x => x.Id == id);
    }

    public IQueryable<Dialog> GetDialogs()
    {
        return _db.Dialogs.AsQueryable();
    }

    public async Task<List<Dialog>?> GetDialogsByOwnerId(long ownerId)
    {
        return await _db.Dialogs.Where(x => x.OwnerId == ownerId).ToListAsync();
    }

    public async Task<bool> Save()
    {
        return await _db.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateDialog(Dialog obj)
    {
        _db.Dialogs.Update(obj);
        return await Save();
    }
}

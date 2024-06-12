using DialogService.Database.Models;

namespace DialogService.Repositories;

//FIXME: Only operational for the user as of now
public interface IDialogRepository
{
    public Task<bool> AddDialog(Dialog obj);
    public Task<bool> UpdateDialog(Dialog obj);
    public Task<bool> DeleteDialog(Dialog obj);
    public Task<Dialog?> GetDialogById(long id);
    public Task<List<Dialog>?> GetDialogsByOwnerId(long ownerId); 
    public IQueryable<Dialog> GetDialogs();
    public Task<bool> Save();
}
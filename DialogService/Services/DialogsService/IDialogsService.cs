using DialogService.Database.Models;
using DialogService.Models.Requests;

namespace DialogService.Services.DialogsService;

public interface IDialogsService
{
    public Task<Dialog> CreateDialog(long ownerId, CreateDialogRequest request);
    public Task<List<Message>?> GetDialogMessages(long ownerId, long dialogId);
    public Task<List<Dialog>?> GetDialogsByOwnerId(long ownerId);
    public Task<bool> DeleteDialog(long ownerId, long id);
    public Task<bool> ClearDialog(long ownerId, long id);
}
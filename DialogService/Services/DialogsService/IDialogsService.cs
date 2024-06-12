using DialogService.Database.Models;
using DialogService.Enums;
using DialogService.Models.Requests;

namespace DialogService.Services.DialogsService;

public interface IDialogsService
{
    public Task<Dialog> CreateDialog(long ownerId, Sender accessor, CreateDialogRequest request);
    public Task<List<Message>?> GetDialogMessages(long ownerId, Sender accessor, long dialogId);
    public Task<List<Dialog>?> GetDialogsByOwnerId(long ownerId, Sender accessor);
    public Task<bool> DeleteDialog(long ownerId, Sender accessor, long id);
    public Task<bool> ClearDialog(long ownerId, Sender accessor,long id);
}
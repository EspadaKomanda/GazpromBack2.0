using DialogService.Database.Models;
using DialogService.Models.Requests;

namespace DialogService.Services.DialogsService;

public interface IDialogsService
{
    public Task<Dialog> CreateDialog(CreateDialogRequest request);
    public Task<List<Message>?> GetDialogMessages(GetDialogMessagesRequest dialogId);
    public Task<List<Dialog>?> GetDialogsByOwnerId(GetDialogsByIdRequest getDialogsByIdRequest);
    public Task<bool> DeleteDialog(DeleteDialogRequest deleteDialogRequest);
    public Task<bool> ClearDialog(ClearDialogRequest clearDialogRequest);
}
using DialogService.Database.Models;
using DialogService.Models.Requests;
using DialogService.Repositories;

namespace DialogService.Services.DialogsService;

public class DialogsService(IDialogRepository dialogRepo, IMessageRepository messageRepo, ILogger<DialogsService> logger) : IDialogsService
{
    private readonly IDialogRepository _dialogRepo = dialogRepo;
    private readonly IMessageRepository _messageRepo = messageRepo;
    private readonly ILogger<DialogsService> _logger = logger;

    public async Task<bool> ClearDialog(long ownerId, long id)
    {

        var dialog = await _dialogRepo.GetDialogById(id);

        if (dialog == null || dialog.OwnerId != ownerId)
        {
            _logger.LogError("Dialog {Id} not found for user {OwnerId}", id, ownerId);
            throw new DialogNotFoundException();
        }

        try
        {
            await _messageRepo.ClearAllInDialog(id);
            _logger.LogInformation("Dialog {Id} cleared", id);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while clearing dialog {Id}", id);
            throw;
        }
    }

    public async Task<Dialog> CreateDialog(long ownerId, CreateDialogRequest request)
    {
        try
        {
            var dialog = new Dialog
            {
                Name = request.Name,
                OwnerId = ownerId
            };
            await _dialogRepo.AddDialog(dialog);
            _logger.LogInformation("Dialog {Id} created for user {OwnerId}", dialog.Id, ownerId);
            return dialog;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating dialog for user {OwnerId}", ownerId);
            throw;
        }
    }

    public async Task<bool> DeleteDialog(long ownerId, long id)
    {
        
        var dialog = await _dialogRepo.GetDialogById(id);

        if (dialog == null || dialog.OwnerId != ownerId)
        {
            _logger.LogError("Dialog {Id} not found for user {OwnerId}", id, ownerId);
            throw new DialogNotFoundException();
        }

        try
        {
            await _dialogRepo.DeleteDialog(dialog);
            _logger.LogInformation("Dialog {Id} deleted", id);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting dialog {Id}", id);
            throw;
        }
    }

    public async Task<List<Message>?> GetDialogMessages(long ownerId, long dialogId)
    {

        var dialog = await _dialogRepo.GetDialogById(dialogId);

        if (dialog == null || dialog.OwnerId != ownerId)
        {
            _logger.LogError("Dialog {Id} not found for user {OwnerId}", dialogId, ownerId);
            throw new DialogNotFoundException();
        }

        try
        {
            return await _messageRepo.GetMessagesByDialogId(dialogId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting messages for dialog {Id}", dialogId);
            throw;
        }
    }

    public async Task<List<Dialog>?> GetDialogsByOwnerId(long ownerId)
    {
        try
        {
            return await _dialogRepo.GetDialogsByOwnerId(ownerId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting dialogs for user {OwnerId}", ownerId);
            throw;
        }
    }
}
using DialogService.Database.Models;
using DialogService.Enums;
using DialogService.Models.Requests;
using DialogService.Repositories;

namespace DialogService.Services.DialogsService;

public class DialogsService(IDialogRepository dialogRepo, IMessageRepository messageRepo, ILogger<DialogsService> logger) : IDialogsService
{
    private readonly IDialogRepository _dialogRepo = dialogRepo;
    private readonly IMessageRepository _messageRepo = messageRepo;
    private readonly ILogger<DialogsService> _logger = logger;

    public async Task<bool> ClearDialog(ClearDialogRequest clearDialogRequest)
    {
        var dialog = await _dialogRepo.GetDialogById(clearDialogRequest.Id);

        if (dialog == null || dialog.OwnerId != clearDialogRequest.OwnerId && clearDialogRequest.Accessor == Sender.User)
        {
            _logger.LogError("Dialog {Id} not found for user {OwnerId}", clearDialogRequest.Id, clearDialogRequest.OwnerId);
            throw new DialogNotFoundException();
        }

        try
        {
            await _messageRepo.ClearAllInDialog(clearDialogRequest.Id);
            _logger.LogInformation("Dialog {Id} cleared", clearDialogRequest.Id);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while clearing dialog {Id}", clearDialogRequest.Id);
            throw;
        }
    }

    public async Task<Dialog> CreateDialog(CreateDialogRequest request)
    {
        try
        {
            var dialog = new Dialog
            {
                Name = request.Name,
                OwnerId = request.OwnerId
            };
            await _dialogRepo.AddDialog(dialog);
            _logger.LogInformation("Dialog {Id} created for user {OwnerId}", dialog.Id, request.OwnerId);
            return dialog;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while creating dialog for user {OwnerId}", request.OwnerId);
            throw;
        }
    }

    public async Task<bool> DeleteDialog(DeleteDialogRequest deleteDialogRequest)
    {
        
        var dialog = await _dialogRepo.GetDialogById(deleteDialogRequest.Id);

        if (dialog == null || dialog.OwnerId != deleteDialogRequest.OwnerId && deleteDialogRequest.Accessor == Sender.User)
        {
            _logger.LogError("Dialog {Id} not found for user {OwnerId}", deleteDialogRequest.Id, deleteDialogRequest.OwnerId);
            throw new DialogNotFoundException();
        }

        try
        {
            await _dialogRepo.DeleteDialog(dialog);
            _logger.LogInformation("Dialog {Id} deleted", deleteDialogRequest.Id);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while deleting dialog {Id}", deleteDialogRequest.Id);
            throw;
        }
    }

    public async Task<List<Message>?> GetDialogMessages(GetDialogMessagesRequest getDialogMessagesRequest)
    {

        var dialog = await _dialogRepo.GetDialogById(getDialogMessagesRequest.Id);

        if (dialog == null || dialog.OwnerId != getDialogMessagesRequest.OwnerId && getDialogMessagesRequest.Accessor == Sender.User)
        {
            _logger.LogError("Dialog {Id} not found for user {OwnerId}", getDialogMessagesRequest.Id, getDialogMessagesRequest.OwnerId);
            throw new DialogNotFoundException();
        }

        try
        {
            return await _messageRepo.GetMessagesByDialogId(getDialogMessagesRequest.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting messages for dialog {Id}", getDialogMessagesRequest.Id);
            throw;
        }
    }

    public async Task<List<Dialog>?> GetDialogsByOwnerId(GetDialogsByIdRequest getDialogsByIdRequest)
    {
        try
        {
            return await _dialogRepo.GetDialogsByOwnerId(getDialogsByIdRequest.OwnerId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting dialogs for user {OwnerId}", getDialogsByIdRequest.OwnerId);
            throw;
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace DialogService.Models.Messages.Requests;

public class EditMessageRequest
{
    [Required]
    public long MessageId { get; set; }
    [Required]
    public long DialogId { get; set; }
    public string? Text { get; set; }
    public long? ImageId { get; set; }
}
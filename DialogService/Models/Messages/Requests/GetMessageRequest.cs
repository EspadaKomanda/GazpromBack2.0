using System.ComponentModel.DataAnnotations;

namespace DialogService.Models.Messages.Requests;

public class GetMessageRequest
{
    [Required]
    public long MessageId { get; set; }
    [Required]
    public long DialogId { get; set; }
}
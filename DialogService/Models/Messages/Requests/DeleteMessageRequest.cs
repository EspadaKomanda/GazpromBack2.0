using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DialogService.Models.Messages.Requests;

public class DeleteMessageRequest
{
    [Required]
    public long MessageId { get; set; }
    [Required]
    public long DialogId { get; set; }
}
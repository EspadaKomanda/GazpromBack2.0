using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DialogService.Enums;

namespace DialogService.Models.Messages.Requests;

public class DeleteMessageRequest
{
    [Required]
    public long OwnerId { get; set;}
    [Required]
    public Sender Accessor { get; set;}
    [Required]
    public long MessageId { get; set; }
    [Required]
    public long DialogId { get; set; }
}
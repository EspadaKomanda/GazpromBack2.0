using System.ComponentModel.DataAnnotations;
using DialogService.Enums;

namespace DialogService.Models.Messages.Requests;

public class GetMessageRequest
{
    [Required]
    public long OwnerId { get; set;}
    [Required]
    public Sender Accessor { get; set;} = Sender.User;
    [Required]
    public long MessageId { get; set; }
    [Required]
    public long DialogId { get; set; }
}
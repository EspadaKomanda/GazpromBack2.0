using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using DialogService.Enums;

namespace DialogService.Models.Messages.Requests;

public class SendMessageRequest
{
    [Required]
    public long OwnerId { get; set;}
    [Required]
    public Sender Accessor { get; set;}
    [Required]
    public long DialogId { get; set; }
    public string? Text { get; set; }
    public long? ImageId { get; set; }
    public Sender Sender { get; set; }
    public MessageStyle Style { get; set; } = MessageStyle.Normal;
}
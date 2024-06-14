using System.ComponentModel.DataAnnotations;
using DialogService.Enums;

namespace DialogService.Models.Requests;

public class GetDialogMessagesRequest
{
    [Required]
    public long OwnerId { get; set;}
    
    [Required]
    public Sender Accessor { get; set;}

    [Required]
    public long Id { get; set;}
}

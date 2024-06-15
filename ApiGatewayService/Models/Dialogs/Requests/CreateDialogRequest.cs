using System.ComponentModel.DataAnnotations;
using DialogService.Enums;

namespace DialogService.Models.Requests;

public class CreateDialogRequest
{
    [Required]
    public long OwnerId { get; set;}
    [Required]
    public Sender Accessor { get; set;}
    [Required]
    public string Name { get; set; } = null!;
}
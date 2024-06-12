using System.ComponentModel.DataAnnotations;

namespace DialogService.Models.Requests;

public class CreateDialogRequest
{
    [Required]
    public string Name { get; set; } = Guid.NewGuid().ToString();
}
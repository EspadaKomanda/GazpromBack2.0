using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Smtp;

public class EmailModel
{
    public string? Subject {get;set;} = null!;

    [Required]
    public string Body {get;set;} = null!;
    
    [Required]
    [EmailAddress]
    public string To {get;set;} = null!;
}
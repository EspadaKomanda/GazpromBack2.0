using System.ComponentModel.DataAnnotations;
using BackGazprom.Validation.Attributes;

namespace BackGazprom.Database.Models;

public class RegistrationCode 
{
    [Key]
    public long Id { get; set; }
    [ValidGuid]
    public string Code { get; set; } = null!;
}
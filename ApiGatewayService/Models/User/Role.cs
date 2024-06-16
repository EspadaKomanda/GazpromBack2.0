using System.ComponentModel.DataAnnotations;

public class Role
{
    [Key]
    public long Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    public bool? Protected { get; set; } = false;
}
using System.ComponentModel.DataAnnotations;

namespace DialogService.Database.Models;

public class Message
{
    [Key]
    public long Id { get; set; }
    [Required]
    public string Text { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public long? ImageId { get; set; }
    [Required]
    public Dialog Dialog { get; set; } = null!;
    public long DialogId { get; set; }
}
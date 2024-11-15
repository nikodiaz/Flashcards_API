using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flashcards.Models;

public class Flashcard
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [StringLength(50)]
    public string UserId { get; set; } = default!;
    [StringLength(100)]
    public string? Word { get; set; }
    [StringLength(500)]
    public string? Definition  { get; set; }
    [StringLength(300)]
    public string? Example { get; set; }
    public int Level { get; set; } = 1;
    public int? LastReviewScore { get; set; }
    public DateTime NextReviewDate { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
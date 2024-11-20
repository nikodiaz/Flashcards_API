using System.ComponentModel.DataAnnotations;

namespace Flashcards.DTO;

public class FlashcardInsertDto
{
    [Required]
    [MaxLength(100)]
    public string Word { get; set; } = default!;
    [MaxLength(500)]
    public string? Definition  { get; set; }
    [MaxLength(300)]
    public string? Example { get; set; }
}
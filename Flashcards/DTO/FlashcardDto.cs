namespace Flashcards.DTO;

public class FlashcardDto
{
    public Guid Id { get; set; }
    public string Word { get; set; } = default!;
    public string? Definition  { get; set; }
    public string? Example  { get; set; }
    public int? LastReviewScore  { get; set; }
    public DateTime NextReviewDate  { get; set; }
}
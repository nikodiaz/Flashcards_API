using Flashcards.DTO;
using Flashcards.Models;

namespace Flashcards.Services;

public interface IFlashcardService
{
    public Task<List<FlashcardDto>> GetAll(string userId);
    public Task<FlashcardDto?> GetById(Guid flashcardId);
    public Task<FlashcardDto> Add(FlashcardInsertDto flashcard, string userId);
    public Task<FlashcardDto?> Update(FlashcardUpdateDto flashcard, Guid flashcardId);
    public Task<string?> Delete(Guid flashcardId);
}
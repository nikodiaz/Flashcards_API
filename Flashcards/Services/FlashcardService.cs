using Flashcards.Data;
using Flashcards.DTO;
using Flashcards.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.Services;

public class FlashcardService : IFlashcardService
{
    private readonly DataContext _context;

    public FlashcardService(DataContext context)
    {
        _context = context;
    }

    public async Task<List<FlashcardDto>> GetAll(string userId)
    {
        var flashcards = await _context.Flashcards
            .Where(fl => fl.UserId == userId)
            .Select(f => 
                new FlashcardDto
                {
                    Id = f.Id,
                    Word = f.Word,
                    Definition = f.Definition,
                    Example = f.Example,
                    LastReviewScore = f.LastReviewScore,
                    NextReviewDate = f.NextReviewDate,
                }).ToListAsync();

        return flashcards;
    }

    public async Task<FlashcardDto?> GetById(Guid flashcardId)
    {
        var card = await _context.Flashcards.SingleOrDefaultAsync(f => f.Id == flashcardId);

        if (card == null)
        {
            return null;
        }
        
        return new FlashcardDto
        {
            Id = card.Id,
            Word = card.Word,
            Definition = card.Definition,
            Example = card.Example,
            LastReviewScore = card.LastReviewScore,
            NextReviewDate = card.NextReviewDate,
        };
    }

    public async Task<FlashcardDto> Add(FlashcardInsertDto flashcardInsertDto, string userId)
    {
        var flashcard = new Flashcard
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Word = flashcardInsertDto.Word,
            Definition = flashcardInsertDto.Definition,
            Example = flashcardInsertDto.Example,
            NextReviewDate = DateTime.UtcNow.AddDays(1),
        };
        
        _context.Flashcards.Add(flashcard);
        await _context.SaveChangesAsync();

        return new FlashcardDto
        {
            Id = flashcard.Id,
            Word = flashcard.Word,
            Definition = flashcard.Definition,
            Example = flashcard.Example,
            LastReviewScore = flashcard.LastReviewScore,
            NextReviewDate = flashcard.NextReviewDate,
        };
    }

    public async Task<FlashcardDto?> Update(FlashcardUpdateDto flashcardUpdateDto, Guid id)
    {
        var card = await _context.Flashcards.FirstOrDefaultAsync(f => f.Id == id);
        if (card == null)
        {
            return null;
        }
        
        card.Word = flashcardUpdateDto.Word ?? card.Word;
        card.Definition = flashcardUpdateDto.Definition;
        card.Example = flashcardUpdateDto.Example;
        
        _context.Attach(card);
        _context.Entry(card).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return new FlashcardDto
        {
            Id = card.Id,
            Word = card.Word,
            Definition = card.Definition,
            Example = card.Example,
            LastReviewScore = card.LastReviewScore,
            NextReviewDate = card.NextReviewDate,
        };
    }

    public async Task<string?> Delete(Guid id)
    {
        var card = await _context.Flashcards.SingleOrDefaultAsync(f => f.Id == id);
        if (card == null)
        {
            return null;
        }

        return "Flashcard deleted";
    }
}
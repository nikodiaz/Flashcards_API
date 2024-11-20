using System.Security.Claims;
using Flashcards.Data;
using Flashcards.DTO;
using Flashcards.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlashcardsController : ControllerBase
{
    
    private readonly DataContext _context;

    public FlashcardsController(DataContext context)
    {
        _context = context;
    }
    
    [HttpGet("all")]
    public async Task<IEnumerable<FlashcardDto>> GetAllFlashcards()
    {
        var userId = User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Enumerable.Empty<FlashcardDto>();
        }
        
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
    
    [HttpPost]
    public async Task<IActionResult> AddFlashcard([FromBody] FlashcardInsertDto flashcardInsertDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
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
        
        return Ok(flashcard);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateFlashcard([FromBody] Flashcard flashcard)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteFlashcard([FromBody] Flashcard flashcard)
    {
        throw new NotImplementedException();
    }
}
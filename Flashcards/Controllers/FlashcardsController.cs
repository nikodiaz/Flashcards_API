using System.Security.Claims;
using Flashcards.Data;
using Flashcards.DTO;
using Flashcards.Models;
using Flashcards.Services;
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
    private readonly IFlashcardService _flashcardService;

    public FlashcardsController(DataContext context, IFlashcardService flashcardService)
    {
        _context = context;
        _flashcardService = flashcardService;
    }
    
    [HttpGet("all")]
    public async Task<IEnumerable<FlashcardDto>> GetAllFlashcards()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Enumerable.Empty<FlashcardDto>();
        }

        return await _flashcardService.GetAll(userId);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FlashcardDto>> GetFlashcardById(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var card = await _flashcardService.GetById(id);
        
        if (card == null)
        {
            return NotFound();
        }
        
        return Ok(card);
    }
    
    [HttpPost]
    public async Task<ActionResult<FlashcardDto>> AddFlashcard([FromBody] FlashcardInsertDto flashcardInsertDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var flashcard = await _flashcardService.Add(flashcardInsertDto, userId);
        
        return Ok(flashcard);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFlashcard(Guid id, [FromBody] FlashcardUpdateDto flashcardUpdateDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var card = await _flashcardService.Update(flashcardUpdateDto, id);
        
        if (card == null)
        {
            return NotFound();
        }
            
        return Ok(card);
        
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFlashcard(Guid id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var card = await _flashcardService.Delete(id);
        if (card == null)
        {
            return NotFound();
        }
        
        return Ok(card);
    }
}
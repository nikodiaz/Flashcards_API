using Flashcards.DTO;
using Flashcards.Models;
using Flashcards.Services;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
   private readonly IAuthService _authService;
   public AuthController(IAuthService authService)
   {
      _authService = authService;
   }

   [HttpPost("register")]
   public async Task<ActionResult<User>> Register([FromBody] RegisterDto request)
   {
      var result = await _authService.Register(request);
      if (!result.Succeeded)
      {
         return BadRequest(result.Errors);
      }
      
      return Created();
   }

   [HttpPost("login")]
   public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginDto request)
   {
      var result = await _authService.Login(request);
      if (result.Success == false)
      {
         return BadRequest(result.ErrorMessage);
      }

      return Ok(result);
   }
}
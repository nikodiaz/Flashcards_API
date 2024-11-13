using Flashcards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
   private readonly UserManager<User> _userManager;
   private readonly SignInManager<User> _signInManager;

   public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
   {
      _userManager = userManager;
      _signInManager = signInManager;
   }
   
   [HttpPost("register")]
   public async Task Register([FromBody] User request){}
}
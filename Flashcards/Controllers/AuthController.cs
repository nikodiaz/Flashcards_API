using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Flashcards.DTO;
using Flashcards.Models;
using Flashcards.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Flashcards.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
   private readonly IAuthRepository _authRepository;
   private readonly IConfiguration _configuration;
   public AuthController(IAuthRepository authRepository, IConfiguration configuration)
   {
      _authRepository = authRepository;
      _configuration = configuration;
   }

   [HttpPost("register")]
   public async Task<ActionResult<User>> Register([FromBody] RegisterDto request)
   {
      var existingUser = await _authRepository.GetUserByEmailAsync(request.Email);
      if (existingUser != null)
      {
         return BadRequest("User already exists");
      }

      var user = new User
      {
         UserName = request.Username,
         Email = request.Email
      };
      
      await _authRepository.RegisterUserAsync(user, request.Password);
      
      return user;
   }

   public async Task<LoginResponse> Login([FromBody] LoginDto request)
   {
      var user = await _authRepository.GetUserByEmailAsync(request.Email);
      if (user == null)
      {
         var res = new LoginResponse
         {
            Success = false,
            ErrorMessage = "User does not exist",
         };

         return res;
      }
      
      var passwordValid = await _authRepository.PasswordSignInAsync(user, request.Password, false);
      if (!passwordValid.Succeeded)
      {
         var res = new LoginResponse
         {
            Success = false,
            ErrorMessage = "Wrong username or password",
         };
         
         return res;
      }

      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
      var tokenDescriptor = new SecurityTokenDescriptor
      {
         Subject = new ClaimsIdentity(new Claim[]
         {
            new (ClaimTypes.NameIdentifier, user.Id),
            new (ClaimTypes.Email, user.Email)
            
         }),
         Expires = DateTime.UtcNow.AddDays(3),
         Issuer = _configuration["JWT:Issuer"],
         Audience = _configuration["JWT:Audience"],
         SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };
      
      var token = tokenHandler.CreateToken(tokenDescriptor);
      var tokenString = tokenHandler.WriteToken(token);

      var response = new LoginResponse
      {
         Success = true,
         Token = tokenString,
         Expiration = token.ValidTo
      };
      
      return response;
   }
}
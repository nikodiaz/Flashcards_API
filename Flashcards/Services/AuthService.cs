using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Flashcards.DTO;
using Flashcards.Models;
using Flashcards.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Flashcards.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IAuthRepository authRepository, IConfiguration configuration)
    {
        _authRepository = authRepository;
        _configuration = configuration;
    }

    public async Task<IdentityResult> Register(RegisterDto registerDto)
    {
        var existingUser = await _authRepository.GetUserByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = $"Email {registerDto.Email} already exists",
                Code = "AccountAlreadyExist"
            });
        }
        
        if (registerDto.Username.Length <= 3)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Description = "Username must be at least 3 characters long",
                Code = "UserNameEmpty"
            });
        }
        
        if (registerDto.Password.Length < 8)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "PasswordTooShort",
                Description = "Password must be at least 8 characters long."
            });
        }
        
        var user = new User
        {
            Email = registerDto.Email,
            UserName = registerDto.Username,
        };
        
        return await _authRepository.RegisterUserAsync(user, registerDto.Password);
    }

    public async Task<LoginResponse> Login(LoginDto loginDto)
    {
        var user = await _authRepository.GetUserByEmailAsync(loginDto.Email);
        if (user == null)
        {
            return new LoginResponse
            {
                Success = false,
                ErrorMessage = "User not found",
            };
        }
        
        var passwordValid = await _authRepository.PasswordSignInAsync(user, loginDto.Password,false);
        if (!passwordValid.Succeeded)
        {
            return new LoginResponse
            {
                Success = false,
                ErrorMessage = "Invalid password",
            };
        }
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor()
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
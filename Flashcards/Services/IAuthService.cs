using Flashcards.DTO;
using Flashcards.Models;
using Microsoft.AspNetCore.Identity;

namespace Flashcards.Services;

public interface IAuthService
{
    Task<IdentityResult> Register(RegisterDto registerDto);
    Task<LoginResponse> Login(LoginDto loginDto);
}
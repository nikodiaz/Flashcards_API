using Flashcards.Models;
using Microsoft.AspNetCore.Identity;

namespace Flashcards.Repository;

public class AuthRepository : IAuthRepository
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthRepository(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<User> GetUserByEmailAsync(string email) =>
        await _userManager.FindByEmailAsync(email);

    public async Task<IdentityResult> RegisterUserAsync(User user, string password) =>
        await _userManager.CreateAsync(user, password);
    
    public async Task<SignInResult> PasswordSignInAsync(User user, string password, bool lockoutOnFailure) =>
        await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
}
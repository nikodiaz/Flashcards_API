using Flashcards.Models;
using Microsoft.AspNetCore.Identity;

namespace Flashcards.Repository;

public interface IAuthRepository
{
    public Task<User> GetUserByEmailAsync(string email);
    public Task<IdentityResult> RegisterUserAsync(User user, string password);
    public Task<SignInResult> PasswordSignInAsync(User user, string password, bool lockoutOnFailure);
}
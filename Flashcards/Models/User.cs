using Microsoft.AspNetCore.Identity;

namespace Flashcards.Models;

public class User : IdentityUser
{
    public ICollection<Flashcard>? Flashcards { get; set; }
}
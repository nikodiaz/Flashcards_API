using Flashcards.DTO;
using Flashcards.Models;
using Moq;
using Flashcards.Repository;
using Flashcards.Services;
using Microsoft.AspNetCore.Identity;

namespace Flashcards.Tests.Services;

public class AuthServiceTest
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTest()
    {
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(config => 
            config["JWT:Key"]).Returns("your_test_key_here_1234567890_undef");
        _configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("your_test_issuer");
        _configurationMock.Setup(config => config["Jwt:Audience"]).Returns("your_test_audience");
        _authService = new AuthService(_authRepositoryMock.Object, _configurationMock.Object);
    }
    
    [Fact]
    public async Task Register_CorrectCredentials_Success()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password1!",
            Username = "Pepe"
        }; 
      
        _authRepositoryMock
            .Setup(repo => repo.RegisterUserAsync(It.IsAny<User>(), "Password1!"))
            .ReturnsAsync(IdentityResult.Success);
     
        // Act
        var result = await _authService.Register(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Succeeded);
        _authRepositoryMock.Verify(repo => 
            repo.RegisterUserAsync(It.IsAny<User>(), registerDto.Password), Times.Once);
    }

    [Fact]
    public async Task Register_InvalidEmail_ReturnError()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test",
            Password = "Password1!",
            Username = "Pepe"
        };

        _authRepositoryMock
            .Setup(repo =>
                repo.RegisterUserAsync(It.IsAny<User>(), "Password1!"))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        var result = await _authService.Register(registerDto);

        // Assert
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Register_EmptyEmail_ReturnError()
    {
         // Arrange
        var registerDto = new RegisterDto
        {
            Email = "",
            Password = "Password1!",
            Username = "Pepe"
        };

        _authRepositoryMock
            .Setup(repo =>
                repo.RegisterUserAsync(It.IsAny<User>(), "Password1!"))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        var result = await _authService.Register(registerDto);

        // Assert
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnError()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password1!",
            Username = "Pepe"
        }; 
      
        _authRepositoryMock
            .Setup(repo => repo.GetUserByEmailAsync(registerDto.Email))
            .ReturnsAsync(new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            });
        
        // Act
        var result = await _authService.Register(registerDto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "DuplicateEmail");
        _authRepositoryMock
                    .Verify(repo => 
                        repo.RegisterUserAsync(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Register_UserNameEmpty_ReturnError()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "Password1!",
            Username = ""
        };
        
        // Act
        var result = await _authService.Register(registerDto);
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "UserNameEmpty");
    }
    
    [Fact]
    public async Task Register_PasswordEmpty_ReturnError()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Email = "test@example.com",
            Password = "",
            Username = "Pepe"
        };
        
        // Act
        var result = await _authService.Register(registerDto);
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "PasswordTooShort");
    }

    [Fact]
    public async Task Login_CorrectCredentials_Success()
    {
        //Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password1!",
        };

        var existingUser = new User
        {
            UserName = loginDto.Email,
            Email = loginDto.Email,
        };

        _authRepositoryMock.Setup(repo =>
            repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(existingUser);

        _authRepositoryMock.Setup(repo => 
            repo.PasswordSignInAsync(existingUser, loginDto.Password, false)).ReturnsAsync(SignInResult.Success);
        
        // Act
        var result = await _authService.Login(loginDto);
        
        // Assert
        Assert.Null(result.ErrorMessage);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task Login_AccountNotExistent_ReturnError()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password1!",
        };

        // Act
       var result = await _authService.Login(loginDto);
       
       // Assert
       Assert.Null(result.Token);
       Assert.NotNull(result.ErrorMessage);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnError()
    {
        //Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "Password1!",
        };

        var existingUser = new User
        {
            UserName = loginDto.Email,
            Email = loginDto.Email,
        };

        _authRepositoryMock.Setup(repo =>
            repo.GetUserByEmailAsync(loginDto.Email)).ReturnsAsync(existingUser);

        _authRepositoryMock.Setup(repo => 
            repo.PasswordSignInAsync(existingUser, loginDto.Password,false)).ReturnsAsync(SignInResult.Failed);
        
        // Act
        var result = await _authService.Login(loginDto);
        
        // Assert
        Assert.Null(result.Token);
        Assert.NotNull(result.ErrorMessage);
    }
}
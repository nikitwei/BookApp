using System;
using System.Threading.Tasks;
using BookApp.Application.DTOs;
using BookApp.Application.Interfaces;
using BookApp.Application.Services;
using BookApp.Domain.Entities;
using BookApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BookApp.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<IJwtProvider> _mockJwtProvider;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new AppDbContext(options);
        _mockJwtProvider = new Mock<IJwtProvider>();
        _authService = new AuthService(_context, _mockJwtProvider.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task RegisterAsync_Success_ReturnsAuthResponseDto()
    {
        // Arrange
        var dto = new RegisterDto("new@test.com", "Password123!", "New User");
        _mockJwtProvider.Setup(x => x.Generate(It.IsAny<User>())).Returns("mock_token");

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mock_token", result.Token);
        Assert.Equal("New User", result.FullName);
        Assert.Equal("new@test.com", result.Email);
        
        var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == "new@test.com");
        Assert.NotNull(userInDb);
        Assert.Equal("Admin", userInDb.Role);
    }

    [Fact]
    public async Task RegisterAsync_EmailTaken_ThrowsException()
    {
        // Arrange
        _context.Users.Add(new User { Email = "taken@test.com", FullName = "Old User", PasswordHash = "hash" });
        await _context.SaveChangesAsync();
        
        var dto = new RegisterDto("taken@test.com", "Password123!", "New User");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(dto));
        Assert.Equal("Email is already taken.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_Success_ReturnsAuthResponseDto()
    {
        // Arrange
        var password = "Password123!";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Email = "login@test.com", FullName = "Login User", PasswordHash = hash };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        _mockJwtProvider.Setup(x => x.Generate(It.IsAny<User>())).Returns("mock_token");
        var dto = new LoginDto("login@test.com", password);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mock_token", result.Token);
        Assert.Equal("Login User", result.FullName);
        Assert.Equal("login@test.com", result.Email);
    }

    [Fact]
    public async Task LoginAsync_InvalidEmail_ThrowsException()
    {
        // Arrange
        var dto = new LoginDto("wrong@test.com", "Password123!");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(dto));
        Assert.Equal("Invalid credentials.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ThrowsException()
    {
        // Arrange
        var password = "Password123!";
        var hash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User { Email = "login@test.com", FullName = "Login User", PasswordHash = hash };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        var dto = new LoginDto("login@test.com", "WrongPassword!");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(dto));
        Assert.Equal("Invalid credentials.", exception.Message);
    }
}

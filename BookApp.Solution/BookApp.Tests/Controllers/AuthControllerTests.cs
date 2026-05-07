using System;
using System.Threading.Tasks;
using BookApp.Api.Controllers;
using BookApp.Application.DTOs;
using BookApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookApp.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    [Fact]
    public async Task Register_ReturnsOk_OnSuccess()
    {
        // Arrange
        var dto = new RegisterDto("test@test.com", "pass", "Test");
        var responseDto = new AuthResponseDto("token", "Test", "test@test.com");
        _mockAuthService.Setup(x => x.RegisterAsync(dto)).ReturnsAsync(responseDto);

        // Act
        var result = await _controller.Register(dto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(responseDto, result.Value);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_OnException()
    {
        // Arrange
        var dto = new RegisterDto("test@test.com", "pass", "Test");
        _mockAuthService.Setup(x => x.RegisterAsync(dto)).ThrowsAsync(new Exception("Email is already taken."));

        // Act
        var result = await _controller.Register(dto) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsOk_OnSuccess()
    {
        // Arrange
        var dto = new LoginDto("test@test.com", "pass");
        var responseDto = new AuthResponseDto("token", "Test", "test@test.com");
        _mockAuthService.Setup(x => x.LoginAsync(dto)).ReturnsAsync(responseDto);

        // Act
        var result = await _controller.Login(dto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(responseDto, result.Value);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_OnException()
    {
        // Arrange
        var dto = new LoginDto("test@test.com", "pass");
        _mockAuthService.Setup(x => x.LoginAsync(dto)).ThrowsAsync(new Exception("Invalid credentials."));

        // Act
        var result = await _controller.Login(dto) as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(401, result.StatusCode);
    }
}

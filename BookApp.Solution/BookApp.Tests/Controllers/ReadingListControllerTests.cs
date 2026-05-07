using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BookApp.Api.Controllers;
using BookApp.Application.DTOs;
using BookApp.Application.Services;
using BookApp.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookApp.Tests.Controllers;

public class ReadingListControllerTests
{
    private readonly Mock<IReadingListService> _mockReadingListService;
    private readonly ReadingListController _controller;
    private readonly Guid _userId;

    public ReadingListControllerTests()
    {
        _mockReadingListService = new Mock<IReadingListService>();
        _controller = new ReadingListController(_mockReadingListService.Object);

        // Mock HttpContext to simulate logged in user
        _userId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, _userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        // Arrange
        var items = new List<UserBookDto> { new UserBookDto(Guid.NewGuid(), Guid.NewGuid(), "Title", "Author", ReadingStatus.WantToRead, 0, DateTime.UtcNow) };
        _mockReadingListService.Setup(x => x.GetUserListAsync(_userId)).ReturnsAsync(items);

        // Act
        var result = await _controller.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(items, result.Value);
    }

    [Fact]
    public async Task Add_ReturnsOk_OnSuccess()
    {
        // Arrange
        var dto = new AddUserBookDto(Guid.NewGuid(), ReadingStatus.WantToRead);
        var createdItem = new UserBookDto(Guid.NewGuid(), dto.BookId, "Title", "Author", dto.Status, 0, DateTime.UtcNow);
        _mockReadingListService.Setup(x => x.AddToListAsync(_userId, dto)).ReturnsAsync(createdItem);

        // Act
        var result = await _controller.Add(dto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(createdItem, result.Value);
    }

    [Fact]
    public async Task Add_ReturnsBadRequest_OnException()
    {
        // Arrange
        var dto = new AddUserBookDto(Guid.NewGuid(), ReadingStatus.WantToRead);
        _mockReadingListService.Setup(x => x.AddToListAsync(_userId, dto)).ThrowsAsync(new Exception("Error message"));

        // Act
        var result = await _controller.Add(dto) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsOk_OnSuccess()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var dto = new UpdateUserBookDto(ReadingStatus.Reading, 4);
        var updatedItem = new UserBookDto(itemId, Guid.NewGuid(), "Title", "Author", dto.Status, dto.Rating, DateTime.UtcNow);
        _mockReadingListService.Setup(x => x.UpdateListItemAsync(_userId, itemId, dto)).ReturnsAsync(updatedItem);

        // Act
        var result = await _controller.Update(itemId, dto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(updatedItem, result.Value);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_OnException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var dto = new UpdateUserBookDto(ReadingStatus.Reading, 4);
        _mockReadingListService.Setup(x => x.UpdateListItemAsync(_userId, itemId, dto)).ThrowsAsync(new Exception("Not found"));

        // Act
        var result = await _controller.Update(itemId, dto) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_OnSuccess()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockReadingListService.Setup(x => x.RemoveFromListAsync(_userId, itemId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(itemId) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(204, result.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequest_OnException()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _mockReadingListService.Setup(x => x.RemoveFromListAsync(_userId, itemId)).ThrowsAsync(new Exception("Not found"));

        // Act
        var result = await _controller.Delete(itemId) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }
}

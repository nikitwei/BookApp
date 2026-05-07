using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookApp.Api.Controllers;
using BookApp.Application.DTOs;
using BookApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookApp.Tests.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _mockBookService;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _mockBookService = new Mock<IBookService>();
        _controller = new BooksController(_mockBookService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithBooks()
    {
        // Arrange
        var books = new List<BookDto> { new BookDto(Guid.NewGuid(), "Title", "Author", "Desc", "123", DateTime.UtcNow) };
        _mockBookService.Setup(x => x.GetAllAsync(1, 10, null)).ReturnsAsync(books);

        // Act
        var result = await _controller.GetAll(1, 10, null) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(books, result.Value);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new BookDto(bookId, "Title", "Author", "Desc", "123", DateTime.UtcNow);
        _mockBookService.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync(book);

        // Act
        var result = await _controller.GetById(bookId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(book, result.Value);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _mockBookService.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync((BookDto?)null);

        // Act
        var result = await _controller.GetById(bookId) as NotFoundResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateBookDto("Title", "Author", "Desc", "123", DateTime.UtcNow);
        var createdBook = new BookDto(Guid.NewGuid(), "Title", "Author", "Desc", "123", DateTime.UtcNow);
        _mockBookService.Setup(x => x.CreateAsync(createDto)).ReturnsAsync(createdBook);

        // Act
        var result = await _controller.Create(createDto) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal(nameof(_controller.GetById), result.ActionName);
        Assert.Equal(createdBook, result.Value);
    }

    [Fact]
    public async Task Update_ReturnsOk_OnSuccess()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var updateDto = new UpdateBookDto("Title", "Author", "Desc", "123", DateTime.UtcNow);
        var updatedBook = new BookDto(bookId, "Title", "Author", "Desc", "123", DateTime.UtcNow);
        _mockBookService.Setup(x => x.UpdateAsync(bookId, updateDto)).ReturnsAsync(updatedBook);

        // Act
        var result = await _controller.Update(bookId, updateDto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(updatedBook, result.Value);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_OnException()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var updateDto = new UpdateBookDto("Title", "Author", "Desc", "123", DateTime.UtcNow);
        _mockBookService.Setup(x => x.UpdateAsync(bookId, updateDto)).ThrowsAsync(new Exception("Not found"));

        // Act
        var result = await _controller.Update(bookId, updateDto) as NotFoundResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_OnSuccess()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _mockBookService.Setup(x => x.DeleteAsync(bookId)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(bookId) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(204, result.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_OnException()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _mockBookService.Setup(x => x.DeleteAsync(bookId)).ThrowsAsync(new Exception("Not found"));

        // Act
        var result = await _controller.Delete(bookId) as NotFoundResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }
}

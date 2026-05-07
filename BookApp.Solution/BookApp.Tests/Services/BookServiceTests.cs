using System;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Application.DTOs;
using BookApp.Application.Services;
using BookApp.Domain.Entities;
using BookApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookApp.Tests.Services;

public class BookServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new AppDbContext(options);
        _bookService = new BookService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPaginatedAndFilteredBooks()
    {
        // Arrange
        _context.Books.Add(new Book { Id = Guid.NewGuid(), Title = "C# Programming", Author = "John Doe", ISBN = "111" });
        _context.Books.Add(new Book { Id = Guid.NewGuid(), Title = "Advanced C#", Author = "Jane Doe", ISBN = "222" });
        _context.Books.Add(new Book { Id = Guid.NewGuid(), Title = "Java Basics", Author = "John Smith", ISBN = "333" });
        await _context.SaveChangesAsync();

        // Act 1: Get all paginated (page 1, size 2)
        var result1 = await _bookService.GetAllAsync(1, 2);
        Assert.Equal(2, result1.Count());

        // Act 2: Filter by search (should match title and author)
        var result2 = await _bookService.GetAllAsync(1, 10, "C#");
        Assert.Equal(2, result2.Count());

        var result3 = await _bookService.GetAllAsync(1, 10, "John");
        Assert.Equal(2, result3.Count()); // John Doe and John Smith
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBook_WhenFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _context.Books.Add(new Book { Id = bookId, Title = "Test Book", Author = "Author", ISBN = "123" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookService.GetByIdAsync(bookId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Book", result!.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _bookService.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_AddsBookToDatabase()
    {
        // Arrange
        var createDto = new CreateBookDto("New Book", "New Author", "Description", "999", DateTime.UtcNow);

        // Act
        var result = await _bookService.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Book", result.Title);
        Assert.Equal(1, await _context.Books.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_UpdatesBook_WhenFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _context.Books.Add(new Book { Id = bookId, Title = "Old Title", Author = "Old Author", ISBN = "123" });
        await _context.SaveChangesAsync();

        var updateDto = new UpdateBookDto("New Title", "New Author", "New Description", "123", DateTime.UtcNow);

        // Act
        var result = await _bookService.UpdateAsync(bookId, updateDto);

        // Assert
        Assert.Equal("New Title", result.Title);
        Assert.Equal("New Author", result.Author);
        
        var bookInDb = await _context.Books.FindAsync(bookId);
        Assert.Equal("New Title", bookInDb!.Title);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsException_WhenNotFound()
    {
        // Arrange
        var updateDto = new UpdateBookDto("New Title", "New Author", "New Description", "123", DateTime.UtcNow);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _bookService.UpdateAsync(Guid.NewGuid(), updateDto));
        Assert.Equal("Book not found", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_DeletesBook_WhenFound()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        _context.Books.Add(new Book { Id = bookId, Title = "Title", Author = "Author", ISBN = "123" });
        await _context.SaveChangesAsync();

        // Act
        await _bookService.DeleteAsync(bookId);

        // Assert
        Assert.Equal(0, await _context.Books.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_ThrowsException_WhenNotFound()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _bookService.DeleteAsync(Guid.NewGuid()));
        Assert.Equal("Book not found", exception.Message);
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Application.DTOs;
using BookApp.Application.Services;
using BookApp.Domain.Entities;
using BookApp.Domain.Enums;
using BookApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookApp.Tests.Services;

public class ReadingListServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ReadingListService _readingListService;

    public ReadingListServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new AppDbContext(options);
        _readingListService = new ReadingListService(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetUserListAsync_ReturnsOnlyUserItems()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        
        _context.Books.Add(new Book { Id = bookId, Title = "Test", Author = "Test" });
        _context.UserBooks.Add(new UserBook { Id = Guid.NewGuid(), UserId = userId, BookId = bookId, Status = ReadingStatus.WantToRead });
        _context.UserBooks.Add(new UserBook { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), BookId = bookId, Status = ReadingStatus.Reading });
        await _context.SaveChangesAsync();

        // Act
        var result = await _readingListService.GetUserListAsync(userId);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task AddToListAsync_Success_AddsItem()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        _context.Books.Add(new Book { Id = bookId, Title = "Test", Author = "Test" });
        await _context.SaveChangesAsync();

        var dto = new AddUserBookDto(bookId, ReadingStatus.WantToRead);

        // Act
        var result = await _readingListService.AddToListAsync(userId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ReadingStatus.WantToRead, result.Status);
        Assert.Equal(0, result.Rating);
        Assert.Equal(1, await _context.UserBooks.CountAsync(ub => ub.UserId == userId));
    }

    [Fact]
    public async Task AddToListAsync_ThrowsException_WhenBookNotFound()
    {
        // Arrange
        var dto = new AddUserBookDto(Guid.NewGuid(), ReadingStatus.WantToRead);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _readingListService.AddToListAsync(Guid.NewGuid(), dto));
        Assert.Equal("Book not found", exception.Message);
    }

    [Fact]
    public async Task AddToListAsync_ThrowsException_WhenBookAlreadyInList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        _context.Books.Add(new Book { Id = bookId, Title = "Test", Author = "Test" });
        _context.UserBooks.Add(new UserBook { UserId = userId, BookId = bookId });
        await _context.SaveChangesAsync();

        var dto = new AddUserBookDto(bookId, ReadingStatus.WantToRead);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _readingListService.AddToListAsync(userId, dto));
        Assert.Equal("Book is already in the reading list", exception.Message);
    }

    [Fact]
    public async Task UpdateListItemAsync_Success_UpdatesStatusAndRating()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userBookId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        _context.Books.Add(new Book { Id = bookId, Title = "Test", Author = "Test" });
        _context.UserBooks.Add(new UserBook { Id = userBookId, UserId = userId, BookId = bookId, Status = ReadingStatus.WantToRead, Rating = 0 });
        await _context.SaveChangesAsync();

        var dto = new UpdateUserBookDto(ReadingStatus.Completed, 5);

        // Act
        var result = await _readingListService.UpdateListItemAsync(userId, userBookId, dto);

        // Assert
        Assert.Equal(ReadingStatus.Completed, result.Status);
        Assert.Equal(5, result.Rating);
        
        var itemInDb = await _context.UserBooks.FindAsync(userBookId);
        Assert.Equal(ReadingStatus.Completed, itemInDb!.Status);
        Assert.Equal(5, itemInDb.Rating);
    }

    [Fact]
    public async Task UpdateListItemAsync_ThrowsException_WhenNotFound()
    {
        // Arrange
        var dto = new UpdateUserBookDto(ReadingStatus.Completed, 5);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _readingListService.UpdateListItemAsync(Guid.NewGuid(), Guid.NewGuid(), dto));
        Assert.Equal("Item not found", exception.Message);
    }

    [Fact]
    public async Task RemoveFromListAsync_Success_DeletesItem()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userBookId = Guid.NewGuid();
        _context.UserBooks.Add(new UserBook { Id = userBookId, UserId = userId });
        await _context.SaveChangesAsync();

        // Act
        await _readingListService.RemoveFromListAsync(userId, userBookId);

        // Assert
        Assert.Equal(0, await _context.UserBooks.CountAsync());
    }

    [Fact]
    public async Task RemoveFromListAsync_ThrowsException_WhenNotFound()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _readingListService.RemoveFromListAsync(Guid.NewGuid(), Guid.NewGuid()));
        Assert.Equal("Item not found", exception.Message);
    }
}

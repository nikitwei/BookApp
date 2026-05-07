using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Application.DTOs;
using BookApp.Application.Interfaces;
using BookApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Application.Services;

public interface IReadingListService
{
    Task<IEnumerable<UserBookDto>> GetUserListAsync(Guid userId);
    Task<UserBookDto> AddToListAsync(Guid userId, AddUserBookDto dto);
    Task<UserBookDto> UpdateListItemAsync(Guid userId, Guid id, UpdateUserBookDto dto);
    Task RemoveFromListAsync(Guid userId, Guid id);
}

public class ReadingListService : IReadingListService
{
    private readonly IAppDbContext _context;

    public ReadingListService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserBookDto>> GetUserListAsync(Guid userId)
    {
        var items = await _context.UserBooks
            .Include(ub => ub.Book)
            .Where(ub => ub.UserId == userId)
            .ToListAsync();

        return items.Select(ub => new UserBookDto(
            ub.Id, ub.BookId, ub.Book!.Title, ub.Book.Author, ub.Status, ub.Rating, ub.AddedAt));
    }

    public async Task<UserBookDto> AddToListAsync(Guid userId, AddUserBookDto dto)
    {
        var book = await _context.Books.FindAsync(dto.BookId);
        if (book == null) throw new Exception("Book not found");

        var existing = await _context.UserBooks.FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == dto.BookId);
        if (existing != null) throw new Exception("Book is already in the reading list");

        var userBook = new UserBook
        {
            UserId = userId,
            BookId = dto.BookId,
            Status = dto.Status,
            Rating = 0
        };

        _context.UserBooks.Add(userBook);
        await _context.SaveChangesAsync();

        return new UserBookDto(userBook.Id, book.Id, book.Title, book.Author, userBook.Status, userBook.Rating, userBook.AddedAt);
    }

    public async Task<UserBookDto> UpdateListItemAsync(Guid userId, Guid id, UpdateUserBookDto dto)
    {
        var item = await _context.UserBooks.Include(ub => ub.Book).FirstOrDefaultAsync(ub => ub.Id == id && ub.UserId == userId);
        if (item == null) throw new Exception("Item not found");

        item.Status = dto.Status;
        item.Rating = dto.Rating;

        await _context.SaveChangesAsync();
        return new UserBookDto(item.Id, item.BookId, item.Book!.Title, item.Book.Author, item.Status, item.Rating, item.AddedAt);
    }

    public async Task RemoveFromListAsync(Guid userId, Guid id)
    {
        var item = await _context.UserBooks.FirstOrDefaultAsync(ub => ub.Id == id && ub.UserId == userId);
        if (item == null) throw new Exception("Item not found");

        _context.UserBooks.Remove(item);
        await _context.SaveChangesAsync();
    }
}

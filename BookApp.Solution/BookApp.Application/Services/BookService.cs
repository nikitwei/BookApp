using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Application.DTOs;
using BookApp.Application.Interfaces;
using BookApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Application.Services;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllAsync(int page = 1, int pageSize = 10, string? search = null);
    Task<BookDto?> GetByIdAsync(Guid id);
    Task<BookDto> CreateAsync(CreateBookDto dto);
    Task<BookDto> UpdateAsync(Guid id, UpdateBookDto dto);
    Task DeleteAsync(Guid id);
}

public class BookService : IBookService
{
    private readonly IAppDbContext _context;

    public BookService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookDto>> GetAllAsync(int page = 1, int pageSize = 10, string? search = null)
    {
        var query = _context.Books.AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
        }

        var books = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return books.Select(b => new BookDto(b.Id, b.Title, b.Author, b.Description, b.ISBN, b.PublishedDate));
    }

    public async Task<BookDto?> GetByIdAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        return book == null ? null : new BookDto(book.Id, book.Title, book.Author, book.Description, book.ISBN, book.PublishedDate);
    }

    public async Task<BookDto> CreateAsync(CreateBookDto dto)
    {
        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            Description = dto.Description,
            ISBN = dto.ISBN,
            PublishedDate = dto.PublishedDate
        };
        
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        
        return new BookDto(book.Id, book.Title, book.Author, book.Description, book.ISBN, book.PublishedDate);
    }

    public async Task<BookDto> UpdateAsync(Guid id, UpdateBookDto dto)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) throw new Exception("Book not found");

        book.Title = dto.Title;
        book.Author = dto.Author;
        book.Description = dto.Description;
        book.ISBN = dto.ISBN;
        book.PublishedDate = dto.PublishedDate;

        await _context.SaveChangesAsync();
        return new BookDto(book.Id, book.Title, book.Author, book.Description, book.ISBN, book.PublishedDate);
    }

    public async Task DeleteAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) throw new Exception("Book not found");

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}

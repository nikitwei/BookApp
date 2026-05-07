using System;
using System.Threading.Tasks;
using BookApp.Application.DTOs;
using BookApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var books = await _bookService.GetAllAsync(page, pageSize, search);
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book == null) return NotFound();
        return Ok(book);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
    {
        var book = await _bookService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookDto dto)
    {
        try
        {
            var book = await _bookService.UpdateAsync(id, dto);
            return Ok(book);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception)
        {
            return NotFound();
        }
    }
}

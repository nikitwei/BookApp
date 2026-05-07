using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BookApp.Application.DTOs;
using BookApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookApp.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/reading-list")]
public class ReadingListController : ControllerBase
{
    private readonly IReadingListService _readingListService;

    public ReadingListController(IReadingListService readingListService)
    {
        _readingListService = readingListService;
    }

    private Guid GetUserId()
    {
        var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(idString!);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var items = await _readingListService.GetUserListAsync(GetUserId());
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddUserBookDto dto)
    {
        try
        {
            var item = await _readingListService.AddToListAsync(GetUserId(), dto);
            return Ok(item);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserBookDto dto)
    {
        try
        {
            var item = await _readingListService.UpdateListItemAsync(GetUserId(), id, dto);
            return Ok(item);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _readingListService.RemoveFromListAsync(GetUserId(), id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

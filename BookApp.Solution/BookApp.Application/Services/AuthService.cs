using System;
using System.Threading.Tasks;
using BookApp.Application.DTOs;
using BookApp.Application.Interfaces;
using BookApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Application.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}

public class AuthService : IAuthService
{
    private readonly IAppDbContext _context;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(IAppDbContext context, IJwtProvider jwtProvider)
    {
        _context = context;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingUser != null)
            throw new Exception("Email is already taken.");

        var user = new User
        {
            Email = dto.Email,
            FullName = dto.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = "Admin" // Let's make first users Admin for easier testing, ideally this is controlled
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _jwtProvider.Generate(user);
        return new AuthResponseDto(token, user.FullName, user.Email);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Invalid credentials.");

        var token = _jwtProvider.Generate(user);
        return new AuthResponseDto(token, user.FullName, user.Email);
    }
}

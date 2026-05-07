using BookApp.Domain.Entities;

namespace BookApp.Application.Interfaces;

public interface IJwtProvider
{
    string Generate(User user);
}

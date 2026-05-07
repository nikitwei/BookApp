using System;
using BookApp.Domain.Enums;

namespace BookApp.Application.DTOs;

public record UserBookDto(Guid Id, Guid BookId, string Title, string Author, ReadingStatus Status, int Rating, DateTime AddedAt);
public record AddUserBookDto(Guid BookId, ReadingStatus Status);
public record UpdateUserBookDto(ReadingStatus Status, int Rating);

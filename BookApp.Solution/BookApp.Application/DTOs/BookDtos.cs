using System;

namespace BookApp.Application.DTOs;

public record BookDto(Guid Id, string Title, string Author, string Description, string ISBN, DateTime PublishedDate);
public record CreateBookDto(string Title, string Author, string Description, string ISBN, DateTime PublishedDate);
public record UpdateBookDto(string Title, string Author, string Description, string ISBN, DateTime PublishedDate);

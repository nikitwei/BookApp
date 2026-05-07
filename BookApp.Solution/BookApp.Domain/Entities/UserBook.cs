using System;
using BookApp.Domain.Enums;

namespace BookApp.Domain.Entities;

public class UserBook
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public Guid BookId { get; set; }
    public Book? Book { get; set; }
    
    public ReadingStatus Status { get; set; } = ReadingStatus.WantToRead;
    public int Rating { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

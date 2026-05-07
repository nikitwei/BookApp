using BookApp.Application.Interfaces;
using BookApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Infrastructure;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<UserBook> UserBooks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        builder.Entity<Book>().HasIndex(b => b.ISBN).IsUnique();
        
        builder.Entity<UserBook>()
            .HasOne(ub => ub.User)
            .WithMany()
            .HasForeignKey(ub => ub.UserId);
            
        builder.Entity<UserBook>()
            .HasOne(ub => ub.Book)
            .WithMany()
            .HasForeignKey(ub => ub.BookId);
            
        builder.Entity<UserBook>().HasIndex(ub => new { ub.UserId, ub.BookId }).IsUnique();
    }
}

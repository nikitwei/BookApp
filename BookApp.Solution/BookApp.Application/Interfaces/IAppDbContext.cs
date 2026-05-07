using System.Threading;
using System.Threading.Tasks;
using BookApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Application.Interfaces;

public interface IAppDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Book> Books { get; set; }
    DbSet<UserBook> UserBooks { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

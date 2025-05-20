using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;
using ToDoApp.Infrastructure.Persistence;

namespace ToDoApp.Infrastructure.Repositories;

public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task TransferTasksToArchiveAsync(string userId)
    {
        await _context.TransferTasksToArchiveAsync(userId);
    }

    public async Task TransferTasksToActiveAsync(string userId)
    {
        await _context.TransferTasksToActiveAsync(userId);
    }
}

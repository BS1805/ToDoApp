using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Interfaces;

public interface IUserRepository : IRepository<ApplicationUser>
{
    Task TransferTasksToArchiveAsync(string userId);
    Task TransferTasksToActiveAsync(string userId);
}

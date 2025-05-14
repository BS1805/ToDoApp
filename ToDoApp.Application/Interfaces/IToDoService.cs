using ToDoApp.Application.DTOs;

namespace ToDoApp.Application.Interfaces;

public interface IToDoService
{
    Task<IEnumerable<TaskViewModel>> GetAllToDoItemsAsync();
    Task<PagedListViewModel<TaskViewModel>> GetPagedToDoItemsAsync(string userId, int pageIndex, int pageSize);
    Task<TaskViewModel> GetToDoItemByIdAsync(int id);
    Task AddToDoItemAsync(TaskViewModel item);
    Task UpdateToDoItemAsync(TaskViewModel item);
    Task DeleteToDoItemAsync(int id);
}
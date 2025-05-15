using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Interfaces;

public interface IToDoService
{
    Task<IEnumerable<ToDoItem>> GetAllToDoItems();
    Task<PagedListViewModel<TaskViewModel>> GetPagedToDoItemsAsync(string userId, int pageIndex, int pageSize);
    Task<ToDoItem> GetToDoItemById(int id);
    Task AddToDoItem(ToDoItem item);
    Task UpdateToDoItem(ToDoItem item);
    Task DeleteToDoItem(int id);
}

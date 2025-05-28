using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Interfaces
{
    public interface IToDoService
    {
        Task<object> GetTaskViewModelForUser(int id, string userId);
        Task<object> CreateToDoItem(TaskViewModel model, string userId);
        Task<object> UpdateToDoItem(TaskViewModel model, string userId);
        Task<object> GetToDoItemForUser(int id, string userId);
        Task<object> DeleteToDoItemForUser(int id, string userId);
        Task<object> GetPagedToDoItemsAsync(string userId, int pageIndex, int pageSize);
        Task<object> GetTaskCountForUserAsync(string userId);
        Task<object> GetStatusesAsync();
        Task<object> GetTasksByStatusAsync(string userId, int statusId);
        Task<object> GetDashboardDataAsync(string userId);
        Task<object> GetPagedTasksByStatusAsync(string userId, int statusId, int pageIndex, int pageSize);
    }
}

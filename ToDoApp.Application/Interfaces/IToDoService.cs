using ToDoApp.Application.DTOs;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Interfaces;

public interface IToDoService
{
    Task<TaskViewModel> GetTaskViewModelForUser(int id, string userId);
    Task<ToDoItem> CreateToDoItem(TaskViewModel model, string userId);
    Task<ToDoItem> UpdateToDoItem(TaskViewModel model, string userId);
    Task<ToDoItem> GetToDoItemForUser(int id, string userId);
    Task<bool> DeleteToDoItemForUser(int id, string userId);
    Task<PagedListViewModel<TaskViewModel>> GetPagedToDoItemsAsync(string userId, int pageIndex, int pageSize);
    Task<int> GetTaskCountForUserAsync(string userId);
    Task<IEnumerable<Status>> GetStatusesAsync();
    Task<List<TaskViewModel>> GetTasksByStatusAsync(string userId, int statusId);
    Task<List<DashboardTaskSummaryDto>> GetDashboardDataAsync(string userId);
    Task<PagedListViewModel<TaskViewModel>> GetPagedTasksByStatusAsync(string userId, int statusId, int pageIndex, int pageSize);


}

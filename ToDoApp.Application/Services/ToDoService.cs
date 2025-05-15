using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Services;

public class ToDoService : IToDoService
{
    private readonly IRepository<ToDoItem> _repository;

    public ToDoService(IRepository<ToDoItem> repository)
    {
        _repository = repository;
    }
    public async Task<int> GetTaskCountForUserAsync(string userId)
    {
        var allTasks = await _repository.GetAllAsync();
        return allTasks.Count(task => task.UserId == userId);
    }
    public async Task<TaskViewModel> GetTaskViewModelForUser(int id, string userId)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null || item.UserId != userId)
            throw new UnauthorizedAccessException();

        return new TaskViewModel
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            IsCompleted = item.IsCompleted
        };
    }
    public async Task<ToDoItem> CreateToDoItem(TaskViewModel model, string userId)
    {
        var toDoItem = new ToDoItem
        {
            Title = model.Title,
            Description = model.Description,
            IsCompleted = model.IsCompleted,
            UserId = userId
        };
        await _repository.AddAsync(toDoItem);
        return toDoItem;
    }

    public async Task<ToDoItem> UpdateToDoItem(TaskViewModel model, string userId)
    {
        var item = await _repository.GetByIdAsync(model.Id.Value);
        if (item == null || item.UserId != userId)
            throw new UnauthorizedAccessException();

        item.Title = model.Title;
        item.Description = model.Description;
        item.IsCompleted = model.IsCompleted;

        await _repository.UpdateAsync(item);
        return item;
    }

    public async Task<ToDoItem> GetToDoItemForUser(int id, string userId)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null || item.UserId != userId)
            throw new UnauthorizedAccessException();

        return item;
    }

    public async Task DeleteToDoItemForUser(int id, string userId)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null || item.UserId != userId)
            throw new UnauthorizedAccessException();

        await _repository.DeleteAsync(id);
    }

    public async Task<PagedListViewModel<TaskViewModel>> GetPagedToDoItemsAsync(string userId, int pageIndex, int pageSize)
    {
        var (items, totalCount) = await _repository.GetPaginatedAsync(
            item => item.UserId == userId,
            pageIndex,
            pageSize
        );

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedListViewModel<TaskViewModel>
        {
            Items = items.Select(item => new TaskViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted
            }).ToList(),
            PageIndex = pageIndex,
            TotalPages = totalPages,
            TotalCount = totalCount,
            PageSize = pageSize
        };
    }
}

using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Services;

public class ToDoService : IToDoService
{
    private readonly IRepository<ToDoItem> _repository;
    private readonly IRepository<Status> _statusRepository;

    public ToDoService(IRepository<ToDoItem> repository, IRepository<Status> statusRepository)
    {
        _repository = repository;
        _statusRepository = statusRepository;
    }

    public async Task<IEnumerable<Status>> GetStatusesAsync()
    {
        return await _statusRepository.GetAllAsync();
    }


    public async Task<PagedListViewModel<TaskViewModel>> GetPagedTasksByStatusAsync(string userId, int statusId, int pageIndex, int pageSize)
{
    var (items, totalCount) = await _repository.GetPaginatedAsync(
        t => t.UserId == userId && t.StatusId == statusId,
        pageIndex,
        pageSize,
        includeProperties: "Status"
    );

    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

    return new PagedListViewModel<TaskViewModel>
    {
        Items = items.Select(t => new TaskViewModel
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            StatusId = t.StatusId
        }).ToList(),
        PageIndex = pageIndex,
        TotalPages = totalPages,
        TotalCount = totalCount,
        PageSize = pageSize
    };
}

    public async Task<List<TaskViewModel>> GetTasksByStatusAsync(string userId, int statusId)
    {
        var tasks = await _repository.GetAllAsync(
            t => t.UserId == userId && t.StatusId == statusId,
            includeProperties: "Status"
        );

        return tasks.Select(t => new TaskViewModel
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            StatusId = t.StatusId

        }).ToList();
    }

    public async Task<List<DashboardTaskSummaryDto>> GetDashboardDataAsync(string userId)
    {
        var statuses = await GetStatusesAsync();
        var dashboardData = new List<DashboardTaskSummaryDto>();

        foreach (var status in statuses)
        {
            var count = await _repository.CountAsync(t => t.UserId == userId && t.StatusId == status.Id);
            dashboardData.Add(new DashboardTaskSummaryDto
            {
                StatusId = status.Id,
                StatusName = status.Name,
                TaskCount = count
            });
        }

        return dashboardData;
    }

    public async Task<int> GetTaskCountForUserAsync(string userId)
    {
        return await _repository.CountAsync(t => t.UserId == userId);
    }

    public async Task<TaskViewModel> GetTaskViewModelForUser(int id, string userId)
    {
        var item = await _repository.GetByIdAsync(id, includeProperties: "Status");
        if (item == null || item.UserId != userId)
            throw new UnauthorizedAccessException();

        return new TaskViewModel
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            StatusId = item.StatusId,

        };
    }

    public async Task<ToDoItem> CreateToDoItem(TaskViewModel model, string userId)
    {
        var status = await _statusRepository.GetByIdAsync(model.StatusId);
        if (status == null)
            throw new ArgumentException("Invalid StatusId provided.");

        var toDoItem = new ToDoItem
        {
            Title = model.Title,
            Description = model.Description,
            StatusId = model.StatusId,
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

        var status = await _statusRepository.GetByIdAsync(model.StatusId);
        if (status == null)
            throw new ArgumentException("Invalid StatusId provided.");

        item.Title = model.Title;
        item.Description = model.Description;
        item.StatusId = model.StatusId;

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
            pageSize,
            includeProperties: "Status" 
        );

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedListViewModel<TaskViewModel>
        {
            Items = items.Select(item => new TaskViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                StatusId = item.StatusId

            }).ToList(),
            PageIndex = pageIndex,
            TotalPages = totalPages,
            TotalCount = totalCount,
            PageSize = pageSize
        };
    }

}

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

    public async Task<IEnumerable<TaskViewModel>> GetAllToDoItemsAsync()
    {
        try
        {
            var items = await _repository.GetAllAsync();
            return items.Select(item => new TaskViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted
            });
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving ToDo items.", ex);
        }
    }

    public async Task<PagedListViewModel<TaskViewModel>> GetPagedToDoItemsAsync(string userId, int pageIndex, int pageSize)
    {
        try
        {
            var (items, totalCount) = await _repository.GetPaginatedAsync(
                item => item.UserId == userId,
                pageIndex,
                pageSize
            );

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            else if (totalPages > 0 && pageIndex > totalPages)
            {
                pageIndex = totalPages;
            }

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
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving paged ToDo items.", ex);
        }
    }

    public async Task<TaskViewModel> GetToDoItemByIdAsync(int id)
    {
        try
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null) return null;

            return new TaskViewModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the ToDo item with ID {id}.", ex);
        }
    }

    public async Task AddToDoItemAsync(TaskViewModel model)
    {
        try
        {
            var item = new ToDoItem
            {
                Title = model.Title,
                Description = model.Description,
                IsCompleted = model.IsCompleted,
                UserId = model.Id.ToString() 
            };

            await _repository.AddAsync(item);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding a ToDo item.", ex);
        }
    }

    public async Task UpdateToDoItemAsync(TaskViewModel model)
    {
        try
        {
            var item = await _repository.GetByIdAsync(model.Id.Value);
            if (item == null) throw new Exception("ToDo item not found.");

            item.Title = model.Title;
            item.Description = model.Description;
            item.IsCompleted = model.IsCompleted;

            await _repository.UpdateAsync(item);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the ToDo item.", ex);
        }
    }

    public async Task DeleteToDoItemAsync(int id)
    {
        try
        {
            await _repository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while deleting the ToDo item with ID {id}.", ex);
        }
    }
}

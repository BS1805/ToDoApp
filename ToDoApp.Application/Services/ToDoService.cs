using System.Security.Claims;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Interfaces;
using ToDoApp.Domain.Entities;

namespace ToDoApp.Application.Services;

public class ToDoService
{
    private readonly IRepository<ToDoItem> _repository;

    public ToDoService(IRepository<ToDoItem> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ToDoItem>> GetAllToDoItems()
    {
        try
        {
            return await _repository.GetAllAsync();
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

    public async Task<ToDoItem> GetToDoItemById(int id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while retrieving the ToDo item with ID {id}.", ex);
        }
    }

    public async Task AddToDoItem(ToDoItem item)
    {
        try
        {
            await _repository.AddAsync(item);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding a ToDo item.", ex);
        }
    }

    public async Task UpdateToDoItem(ToDoItem item)
    {
        try
        {
            await _repository.UpdateAsync(item);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the ToDo item.", ex);
        }
    }

    public async Task DeleteToDoItem(int id)
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
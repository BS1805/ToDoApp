using ToDoApp.Models;

namespace ToDoApp.Data;
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
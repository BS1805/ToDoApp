using System.Linq.Expressions;

namespace ToDoApp.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(Expression<Func<T, bool>> filter, int pageIndex, int pageSize);
    Task<IEnumerable<Status>> GetStatusesAsync();
}
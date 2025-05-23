﻿using System.Linq.Expressions;

namespace ToDoApp.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
    Task<int> CountAsync(Expression<Func<T, bool>> filter);
    Task<T> GetByIdAsync(int id, string? includeProperties = null);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<(IEnumerable<T> Items, int TotalCount)> GetPaginatedAsync(
       Expression<Func<T, bool>> filter, int pageIndex, int pageSize, string? includeProperties = null);
    Task<IEnumerable<Status>> GetStatusesAsync();
}

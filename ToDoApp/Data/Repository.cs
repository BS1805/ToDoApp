using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToDoApp.Data;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            return await _dbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            
            throw new Exception("An error occurred while retrieving all entities.", ex);
        }
    }

    public async Task<T> GetByIdAsync(int id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with ID {id} was not found.");
            }
            return entity;
        }
        catch (Exception ex)
        {
            
            throw new Exception($"An error occurred while retrieving the entity with ID {id}.", ex);
        }
    }

    public async Task AddAsync(T entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
           
            throw new Exception("An error occurred while adding the entity.", ex);
        }
    }

    public async Task UpdateAsync(T entity)
    {
        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            
            throw new Exception("An error occurred while updating the entity.", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Entity with ID {id} was not found.");
            }
        }
        catch (Exception ex)
        {
           
            throw new Exception($"An error occurred while deleting the entity with ID {id}.", ex);
        }
    }
}
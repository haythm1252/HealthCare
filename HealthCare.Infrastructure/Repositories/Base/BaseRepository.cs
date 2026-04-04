using HealthCare.Application.Interfaces.Repositories.Base;
using HealthCare.Domain.Entities;
using HealthCare.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Infrastructure.Repositories.Base;

public class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T> where T : class
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();


    public IQueryable<T> AsQueryable() => _dbSet;
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }
    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(criteria, cancellationToken);
    }

    public Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null, CancellationToken cancellationToken = default)
    {
        return criteria == null ? _dbSet.CountAsync(cancellationToken) : _dbSet.CountAsync(criteria, cancellationToken);
    }

    public Task Delete(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }
    public Task DeleteRange(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public Task Update(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
}

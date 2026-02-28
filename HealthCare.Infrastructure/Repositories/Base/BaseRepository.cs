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

public class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context = context;
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
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

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? criteria = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? includes = null, int? skip = null, int? take = null, bool asNoTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (criteria != null)
            query = query.Where(criteria);

        if (includes != null && includes.Any())
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (orderBy != null)
            query = orderBy(query);

        if (asNoTracking)
            query = query.AsNoTracking();

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (take.HasValue)
            query = query.Take(take.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public Task<T?> GetAsync(Expression<Func<T, bool>> criteria, List<Expression<Func<T, object>>>? includes = null, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (includes != null && includes.Any())
            query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (asNoTracking)
            query = query.AsNoTracking();

        return query.FirstOrDefaultAsync(criteria, cancellationToken);
    }

    public Task Update(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }
}

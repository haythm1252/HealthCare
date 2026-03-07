using HealthCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCare.Application.Interfaces.Repositories.Base;

public interface IBaseRepository<T> where T : BaseEntity
{
    IQueryable<T> AsQueryable();
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task Update(T entity);
    Task Delete(T entity);
    Task<bool> AnyAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null, CancellationToken cancellationToken = default);

}
    
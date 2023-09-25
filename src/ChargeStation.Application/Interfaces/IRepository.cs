using ChargeStation.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ChargeStation.Application.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeExpressions);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeExpressions);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}

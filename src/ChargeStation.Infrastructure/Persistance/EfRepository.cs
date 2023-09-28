using ChargeStation.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChargeStation.Domain.Common;

namespace ChargeStation.Infrastructure.Persistance
{
    /// <summary>
    /// This class a an EFCore-based implementation of the <see cref="IRepository{T}"/> interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        public EfRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get by id
        public async Task<T> GetByIdAsync(int id, params string[] includeExpressions)
        {
            IQueryable<T> query = _context.Set<T>().Where(e => e.Id == id);

            if (includeExpressions.Length != 0)
            {
                foreach (var includeExpression in includeExpressions)
                {
                    query = query.Include(includeExpression);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        // Get all
        public async Task<IEnumerable<T>> GetAllAsync(params string[] includeExpressions)
        {
            IQueryable<T> query = _context.Set<T>().AsNoTracking();

            if (includeExpressions.Length != 0)
            {
                foreach (var includeExpression in includeExpressions)
                {
                    query = query.Include(includeExpression);
                }
            }

            return await query.ToListAsync();
        }

        // Add
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // Update
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        // Delete
        public async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

}

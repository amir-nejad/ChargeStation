using ChargeStation.Domain.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeStation.Application.Interfaces
{
    /// <summary>
    /// This interface is a generic definition of the repository pattern that the database interaction services are defined here.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// This method can get a <see cref="T"/> object from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeExpressions"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(int id, params string[] includeExpressions);

        /// <summary>
        /// This method can get all <see cref="T"/> objects from the database.
        /// </summary>
        /// <param name="includeExpressions"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(params string[] includeExpressions);

        /// <summary>
        /// This method can add a <see cref="T"/> entity object to the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task AddAsync(T entity);

        /// <summary>
        /// This method can update a <see cref="T"/> entity object in the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// This method can delete a <see cref="T"/> entity object from the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(T entity);
    }
}

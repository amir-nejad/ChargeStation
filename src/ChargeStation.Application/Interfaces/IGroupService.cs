using ChargeStation.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeStation.Application.Interfaces
{
    /// <summary>
    /// This interface is used for the <see cref="GroupEntity"/> database-related methods definition.
    /// </summary>
    public interface IGroupService
    {
        /// <summary>
        /// This method can get a <see cref="GroupEntity"/> object from the database.
        /// </summary>
        /// <param name="id">The id of the target <see cref="GroupEntity"/> record.</param>
        /// <returns></returns>
        Task<GroupEntity> GetGroupByIdAsync(int id);

        /// <summary>
        /// This method can get a list of <see cref="GroupEntity"/> objects from the database.
        /// </summary>
        /// <returns></returns>
        Task<IList<GroupEntity>> GetGroupsAsync();

        /// <summary>
        /// This method can create a <see cref="GroupEntity"/> record in the database.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        Task CreateGroupAsync(GroupEntity group);

        /// <summary>
        /// This method can update a <see cref="GroupEntity"/> record in the database.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        Task UpdateGroupAsync(GroupEntity group);

        /// <summary>
        /// This method can delete a <see cref="GroupEntity"/> from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteGroupAsync(int id);
    }
}

using ChargeStation.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeStation.Application.Interfaces
{
    /// <summary>
    /// This interface is used for the <see cref="ConnectorEntity"/> database-related methods definition.
    /// </summary>
    public interface IConnectorService
    {
        /// <summary>
        /// This method can get a <see cref="ConnectorEntity"/> object from the database.
        /// </summary>
        /// <param name="id">The id of the target <see cref="ConnectorEntity"/> record.</param>
        /// <returns></returns>
        Task<ConnectorEntity> GetConnectorByIdAsync(int id);

        /// <summary>
        /// This method can get a list of <see cref="ConnectorEntity"/> objects from the database.
        /// </summary>
        /// <returns></returns>
        Task<IList<ConnectorEntity>> GetConnectorsAsync();

        /// <summary>
        /// This method can create a <see cref="ConnectorEntity"/> record in the database.
        /// </summary>
        /// <param name="connector"></param>
        /// <returns></returns>
        Task CreateConnectorAsync(ConnectorEntity connector);

        /// <summary>
        /// This method can update a <see cref="ConnectorEntity"/> record in the database.
        /// </summary>
        /// <param name="connector"></param>
        /// <returns></returns>
        Task UpdateConnectorAsync(ConnectorEntity connector);

        /// <summary>
        /// This method can delete a <see cref="ConnectorEntity"/> from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteConnectorAsync(int id);
    }
}

using ChargeStation.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeStation.Application.Interfaces
{
    /// <summary>
    /// This interface is used for the <see cref="ChargeStationEntity"/> database-related methods definition.
    /// </summary>
    public interface IChargeStationService
    {
        /// <summary>
        /// This method can get a <see cref="ChargeStationEntity"/> object from the database.
        /// </summary>
        /// <param name="id">The id of the target <see cref="ChargeStationEntity"/> record.</param>
        /// <returns></returns>
        Task<ChargeStationEntity> GetChargeStationByIdAsync(int id);

        /// <summary>
        /// This method can get a list of <see cref="ChargeStationEntity"/> objects from the database.
        /// </summary>
        /// <returns></returns>
        Task<IList<ChargeStationEntity>> GetChargeStationsAsync();

        /// <summary>
        /// This method can create a <see cref="ChargeStationEntity"/> record in the database.
        /// </summary>
        /// <param name="chargeStation"></param>
        /// <returns></returns>
        Task CreateChargeStationAsync(ChargeStationEntity chargeStation);

        /// <summary>
        /// This method can update a <see cref="ChargeStationEntity"/> record in the database.
        /// </summary>
        /// <param name="chargeStation"></param>
        /// <returns></returns>
        Task UpdateChargeStationAsync(ChargeStationEntity chargeStation);

        /// <summary>
        /// This method can delete a <see cref="ChargeStationEntity"/> from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteChargeStationAsync(int id);
    }
}

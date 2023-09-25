using ChargeStation.Application.Interfaces;
using ChargeStation.Domain.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeStation.Application.Services
{
    /// <summary>
    /// This class is used as an implementation of the <see cref="IChargeStationService"/> interface.
    /// </summary>
    public class ChargeStationService : IChargeStationService
    {
        private readonly IRepository<ChargeStationEntity> _repository;
        private readonly ILogger _logger;

        public ChargeStationService(IRepository<ChargeStationEntity> repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Create
        public async Task CreateChargeStationAsync(ChargeStationEntity chargeStation)
        {
            try
            {
                await _repository.AddAsync(chargeStation);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message, ex.StackTrace);
            }
        }

        // Get by Id
        public async Task<ChargeStationEntity> GetChargeStationByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id, x => x.Connectors);
        }

        // Get list
        public async Task<IList<ChargeStationEntity>> GetChargeStationsAsync()
        {
            return (IList<ChargeStationEntity>)await _repository.GetAllAsync(x => x.Connectors);
        }

        // Update
        public async Task UpdateChargeStationAsync(ChargeStationEntity chargeStation)
        {
            try
            {
                await _repository.UpdateAsync(chargeStation);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message, ex.StackTrace);
            }
        }

        // Delete
        public async Task DeleteChargeStationAsync(int id)
        {
            try
            {
                var chargeStation = await GetChargeStationByIdAsync(id);

                if(chargeStation is null)
                    throw new KeyNotFoundException(nameof(chargeStation));

                await _repository.DeleteAsync(chargeStation);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message, ex.StackTrace);
            }
        }
    }
}

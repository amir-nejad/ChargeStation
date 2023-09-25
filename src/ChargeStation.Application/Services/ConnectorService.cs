using ChargeStation.Application.Interfaces;
using ChargeStation.Domain.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChargeStation.Application.Services
{
    /// <summary>
    /// This class is used as an implementation of the <see cref="IConnectorService"/> interface.
    /// </summary>
    public class ConnectorService : IConnectorService
    {
        private readonly IRepository<ConnectorEntity> _repository;
        private readonly ILogger _logger;

        public ConnectorService(IRepository<ConnectorEntity> repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Create
        public async Task CreateConnectorAsync(ConnectorEntity chargeStation)
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
        public async Task<ConnectorEntity> GetConnectorByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // Get list
        public async Task<IList<ConnectorEntity>> GetConnectorsAsync()
        {
            return (IList<ConnectorEntity>)await _repository.GetAllAsync();
        }

        // Update
        public async Task UpdateConnectorAsync(ConnectorEntity chargeStation)
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
        public async Task DeleteConnectorAsync(int id)
        {
            try
            {
                var chargeStation = await GetConnectorByIdAsync(id);

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

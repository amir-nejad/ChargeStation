using ChargeStation.Application.Interfaces;
using ChargeStation.Domain.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChargeStation.Application.Services
{
    /// <summary>
    /// This class is used as an implementation of the <see cref="IGroupService"/> interface.
    /// </summary>
    public class GroupService : IGroupService
    {
        private readonly IRepository<GroupEntity> _repository;
        private readonly ILogger _logger;

        public GroupService(IRepository<GroupEntity> repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // Create
        public async Task CreateGroupAsync(GroupEntity chargeStation)
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
        public async Task<GroupEntity> GetGroupByIdAsync(int id)
        {
            return await _repository
                .GetByIdAsync(id, x => 
                    x.ChargeStations,
                    x => x.ChargeStations.Select(c => c.Connectors));
        }

        // Get list
        public async Task<IList<GroupEntity>> GetGroupsAsync()
        {
            return (IList<GroupEntity>)await _repository
                .GetAllAsync(x => 
                    x.ChargeStations, 
                    x => x.ChargeStations.Select(c => c.Connectors));
        }

        // Update
        public async Task UpdateGroupAsync(GroupEntity chargeStation)
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
        public async Task DeleteGroupAsync(int id)
        {
            try
            {
                var chargeStation = await GetGroupByIdAsync(id);

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

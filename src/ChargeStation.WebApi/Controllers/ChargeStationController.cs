using ChargeStation.Application.Interfaces;
using ChargeStation.Application.Services;
using ChargeStation.Domain.Entities;
using ChargeStation.WebApi.Models.Dtos.ChargeStation;
using ChargeStation.WebApi.Models.Dtos.Connector;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ChargeStation.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChargeStationController : ControllerBase
    {
        private readonly IChargeStationService _chargeStationService;
        private readonly ILogger _logger;

        public ChargeStationController(IChargeStationService chargeStationService, ILogger logger)
        {
            _chargeStationService = chargeStationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetChargeStationsAsync()
        {
            return Ok(await _chargeStationService.GetChargeStationsAsync());
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetChargeStationAsync(int? id)
        {
            if (id is null)
                return BadRequest();

            var response = new ChargeStationDto();

            var chargeStationEntity = await _chargeStationService.GetChargeStationByIdAsync(id.Value);

            if (chargeStationEntity is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;

                return new JsonResult(null);
            }

            response.Id = chargeStationEntity.Id;
            response.Name = chargeStationEntity.Name;
            response.CreatedDateUtc = chargeStationEntity.CreatedDateUtc;
            response.LastModifiedDateUtc = chargeStationEntity.LastModifiedDateUtc;

            // Loading connectors
            if (chargeStationEntity.Connectors is not null && chargeStationEntity.Connectors.Count is not 0)
            {
                foreach (var connectorEntity in chargeStationEntity.Connectors)
                {
                    response.Connectors.Add(new ConnectorDto()
                    {
                        AmpsMaxCurrent = connectorEntity.AmpsMaxCurrent,
                        CreatedDateUtc = connectorEntity.CreatedDateUtc,
                        LastModifiedDateUtc = connectorEntity.LastModifiedDateUtc,
                        Id = connectorEntity.Id,
                        ChargeStation = new ChargeStationDto()
                        {
                            Name = connectorEntity.ChargeStation.Name,
                            CreatedDateUtc = connectorEntity.ChargeStation.CreatedDateUtc,
                            LastModifiedDateUtc = connectorEntity.ChargeStation.LastModifiedDateUtc,
                            GroupId = connectorEntity.ChargeStation.GroupId,
                            Id = connectorEntity.ChargeStation.Id
                        }
                    });
                }
            }

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateChargeStationAsync([FromBody] ChargeStationDto chargeStation)
        {
            var chargeStationEntity = new ChargeStationEntity()
            {
                GroupId = chargeStation.GroupId,
                Name = chargeStation.Name,
            };

            var response = new CreateUpdateChargeStationResponseDto();

            await _chargeStationService.CreateChargeStationAsync(chargeStationEntity);

            if (chargeStationEntity.Id == 0)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;

                response.Success = false;
                response.Message = "The create process was failed.";

                return new JsonResult(response);
            }

            response.Success = true;
            chargeStation.Id = chargeStationEntity.Id;

            response.ChargeStation = chargeStation;

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateChargeStationAsync([FromBody] ChargeStationDto chargeStation)
        {
            if (!chargeStation.Id.HasValue)
                return BadRequest();

            var response = new CreateUpdateChargeStationResponseDto();

            var chargeStationEntity = new ChargeStationEntity()
            {
                Id = chargeStation.Id.Value,
                Name = chargeStation.Name,
                GroupId = chargeStation.GroupId,
                CreatedDateUtc = chargeStation.CreatedDateUtc.GetValueOrDefault()
            };

            await _chargeStationService.UpdateChargeStationAsync(chargeStationEntity);

            if (chargeStationEntity is null)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;

                response.Success = false;
                response.Message = "The update process was failed.";

                return new JsonResult(response);
            }

            response.Success = true;
            response.ChargeStation = chargeStation;

            return Ok(response);
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> DeleteChargeStationAsync(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var response = new DeleteChargeStationResponseDto();

            try
            {
                var deleteTask = _chargeStationService.DeleteChargeStationAsync(id.Value);
                await deleteTask; // Wait for the task to complete

                if (deleteTask.Status == TaskStatus.RanToCompletion)
                {
                    response.Success = true;
                    return Ok(response); // Task completed successfully
                }
                else if (deleteTask.Status == TaskStatus.Faulted)
                {
                    Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Success = false;
                    response.Message = "An error occurred while processing the request.";

                    return new JsonResult(response); // Task faulted (exception occurred)
                }
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation if needed
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Success = false;
                response.Message = "The operation was canceled.";

                return new JsonResult(response);
            }

            Response.StatusCode = StatusCodes.Status500InternalServerError;
            response.Success = false;
            response.Message = "An unknown error occurred.";

            return new JsonResult(response);
        }
    }
}

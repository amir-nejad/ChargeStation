using ChargeStation.Application.Interfaces;
using ChargeStation.Domain.Entities;
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
    public class ConnectorController : ControllerBase
    {
        private readonly IConnectorService _connectorService;
        private readonly IChargeStationService _chargeStationService;
        private readonly ILogger _logger;

        public ConnectorController(IConnectorService connectorService, ILogger logger, IChargeStationService chargeStationService)
        {
            _connectorService = connectorService;
            _logger = logger;
            _chargeStationService = chargeStationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetConnectorsAsync()
        {
            return Ok(await _connectorService.GetConnectorsAsync());
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetConnectorAsync(int? id)
        {
            if (id is null)
                return BadRequest();

            var response = new ConnectorDto();

            var connectorEntity = await _connectorService.GetConnectorByIdAsync(id.Value);

            if (connectorEntity is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;

                return new JsonResult(null);
            }

            response.Id = connectorEntity.Id;
            response.AmpsMaxCurrent = connectorEntity.AmpsMaxCurrent;
            response.CreatedDateUtc = connectorEntity.CreatedDateUtc;
            response.LastModifiedDateUtc = connectorEntity.LastModifiedDateUtc;

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConnectorAsync([FromBody] ConnectorDto connector)
        {
            if (connector is null)
                return BadRequest();
            
            var response = new CreateUpdateConnectorResponseDto();

            var chargeStation = await _chargeStationService.GetChargeStationByIdAsync(connector.ChargeStationId);

            if (chargeStation is null)
                return BadRequest();

            // Validating the maximum amount of charge station children. (Max is 5)
            if (chargeStation.Connectors is not null && chargeStation.Connectors.Count + 1 > 5)
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;

                response.Success = false;
                response.Message = "Cannot add more connector to the choiced Charge Station. Please remove other connectors if you want to add a new one.";

                return new JsonResult(response);
            }

            var connectorEntity = new ConnectorEntity()
            {
                AmpsMaxCurrent = connector.AmpsMaxCurrent,
                ChargeStationId = connector.ChargeStationId,
            };


            await _connectorService.CreateConnectorAsync(connectorEntity);

            if (connectorEntity.Id == 0)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;

                response.Success = false;
                response.Message = "The create process was failed.";

                return new JsonResult(response);
            }

            response.Success = true;
            connector.Id = connectorEntity.Id;

            response.Connector = connector;

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateConnectorAsync([FromBody] ConnectorDto connector)
        {
            if (!connector.Id.HasValue)
                return BadRequest();

            var response = new CreateUpdateConnectorResponseDto();

            var connectorEntity = new ConnectorEntity()
            {
                Id = connector.Id.Value,
                CreatedDateUtc = connector.CreatedDateUtc.GetValueOrDefault(),
                AmpsMaxCurrent = connector.AmpsMaxCurrent,
                ChargeStationId = connector.ChargeStationId
            };

            await _connectorService.UpdateConnectorAsync(connectorEntity);

            if (connectorEntity is null)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;

                response.Success = false;
                response.Message = "The update process was failed.";

                return new JsonResult(response);
            }

            response.Success = true;
            response.Connector = connector;

            return Ok(response);
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> DeleteConnectorAsync(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var response = new DeleteConnectorResponseDto();

            try
            {
                var deleteTask = _connectorService.DeleteConnectorAsync(id.Value);
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

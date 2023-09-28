using ChargeStation.Application.Interfaces;
using ChargeStation.Domain.Entities;
using ChargeStation.WebApi.Models.Dtos.ChargeStation;
using ChargeStation.WebApi.Models.Dtos.Connector;
using ChargeStation.WebApi.Models.Dtos.Group;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChargeStation.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly ILogger _logger;

        public GroupController(IGroupService groupService, ILogger logger)
        {
            _groupService = groupService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupsAsync()
        {
            return Ok(await _groupService.GetGroupsAsync());
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetGroupAsync(int? id)
        {
            if (id is null)
                return BadRequest();

            var response = new GroupDto();

            var groupEntity = await _groupService.GetGroupByIdAsync(id.Value);

            if (groupEntity is null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;

                return new JsonResult(null);
            }

            response.Id = groupEntity.Id;
            response.Name = groupEntity.Name;
            response.AmpsCapacity = groupEntity.AmpsCapacity;
            response.CreatedDateUtc = groupEntity.CreatedDateUtc;
            response.LastModifiedDateUtc = groupEntity.LastModifiedDateUtc;
            response.ChargeStations = groupEntity.ChargeStations.Select(cs => new ChargeStationDto()
            {
                Id = cs.Id,
                Name = cs.Name,
                GroupId = cs.GroupId,
                CreatedDateUtc = cs.CreatedDateUtc,
                LastModifiedDateUtc = cs.LastModifiedDateUtc,
                Connectors = cs.Connectors.Select(c => new ConnectorDto()
                {
                    Id = c.Id,
                    ChargeStationId = cs.Id,
                    AmpsMaxCurrent = c.AmpsMaxCurrent,
                    CreatedDateUtc = c.CreatedDateUtc,
                    LastModifiedDateUtc = c.LastModifiedDateUtc
                }).ToList()
            }).ToList();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroupAsync([FromBody] GroupDto group)
        {
            var groupEntity = new GroupEntity()
            {
                AmpsCapacity = group.AmpsCapacity,
                Name = group.Name,
            };

            var response = new CreateUpdateGroupResponseDto();

            await _groupService.CreateGroupAsync(groupEntity);

            if (groupEntity.Id == 0)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;

                response.Success = false;
                response.Message = "The create process was failed.";

                return new JsonResult(response);
            }

            response.Success = true;
            group.Id = groupEntity.Id;

            response.Group = group;

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGroupAsync([FromBody] GroupDto group)
        {
            if (!group.Id.HasValue)
                return BadRequest();

            var response = new CreateUpdateGroupResponseDto();

            var groupEntity = await _groupService.GetGroupByIdAsync(group.Id.Value);

            if (groupEntity is null)
                return NotFound();

            var connectorsAmpsMaxCurrentSum = groupEntity.ChargeStations.Sum(cs => cs.Connectors.Sum(c => c.AmpsMaxCurrent));

            if (group.AmpsCapacity < connectorsAmpsMaxCurrentSum)
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;

                response.Success = false;
                response.Message = "The Amps Capacity of a Group cannot be smaller than all childre'n connectors.";

                return new JsonResult(response);
            }

            groupEntity = new GroupEntity()
            {
                Id = group.Id.Value,
                AmpsCapacity = group.AmpsCapacity,
                Name = group.Name,
                CreatedDateUtc = group.CreatedDateUtc.GetValueOrDefault()
            };

            await _groupService.UpdateGroupAsync(groupEntity);

            if (groupEntity is null)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;

                response.Success = false;
                response.Message = "The update process was failed.";

                return new JsonResult(response);
            }

            response.Success = true;
            response.Group = group;

            return Ok(response);
        }

        [HttpDelete("{id?}")]
        public async Task<IActionResult> DeleteGroupAsync(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var response = new DeleteGroupResponseDto();

            try
            {
                var deleteTask = _groupService.DeleteGroupAsync(id.Value);
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

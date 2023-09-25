using ChargeStation.Application.Interfaces;
using ChargeStation.Domain.Entities;
using ChargeStation.WebApi.Models.Dtos.Group;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading.Tasks;

namespace ChargeStation.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupSerice;
        private readonly ILogger _logger;

        public GroupController(IGroupService groupService, ILogger logger)
        {
            _groupSerice = groupService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetGroupsAsync()
        {
            return Ok(await _groupSerice.GetGroupsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroupById(int? id)
        {
            if (id is null)
                return BadRequest();

            var response = new GroupDto();

            var groupEntity = await _groupSerice.GetGroupByIdAsync(id.Value);

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

            await _groupSerice.CreateGroupAsync(groupEntity);

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

            var groupEntity = new GroupEntity()
            {
                Id = group.Id.Value,
                AmpsCapacity = group.AmpsCapacity,
                Name = group.Name,
                CreatedDateUtc = group.CreatedDateUtc.GetValueOrDefault()
            };

            await _groupSerice.UpdateGroupAsync(groupEntity);

            if (groupEntity is null)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;

                response.Success = false;
                response.Message = "The update process was failed.";

                return new JsonResult(response);
            }

            response.Success = true;
            response.Group = group;

            return Ok();
        }
    }
}

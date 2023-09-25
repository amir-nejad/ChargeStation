using Newtonsoft.Json;

namespace ChargeStation.WebApi.Models.Dtos.Group
{
    public record CreateUpdateGroupResponseDto : BaseResponse
    {
        [JsonProperty("group")]
        public GroupDto Group { get; set; }
    }
}

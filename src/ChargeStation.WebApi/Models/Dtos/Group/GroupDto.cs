using Newtonsoft.Json;

namespace ChargeStation.WebApi.Models.Dtos.Group
{
    public record GroupDto : BaseDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ampsCapacity")]
        public int AmpsCapacity { get; set; }
    }
}

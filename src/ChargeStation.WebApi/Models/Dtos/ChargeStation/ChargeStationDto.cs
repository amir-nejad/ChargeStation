using ChargeStation.WebApi.Models.Dtos.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ChargeStation.WebApi.Models.Dtos.ChargeStation
{
    public record ChargeStationDto : BaseDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("groupId")]
        public int GroupId { get; set; }

        [JsonProperty("ampsCapacity")]
        public IList<ConnectorDto> Connectors { get; set; }
    }
}

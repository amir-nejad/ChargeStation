using ChargeStation.WebApi.Models.Dtos.ChargeStation;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace ChargeStation.WebApi.Models.Dtos.Group
{
    public record GroupDto : BaseDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ampsCapacity")]
        public int AmpsCapacity { get; set; }

        [JsonProperty("chargeStations")]
        public IList<ChargeStationDto> ChargeStations { get; set; }
    }
}

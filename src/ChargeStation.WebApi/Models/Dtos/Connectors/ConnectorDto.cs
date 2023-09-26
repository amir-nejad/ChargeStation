using Newtonsoft.Json;

namespace ChargeStation.WebApi.Models.Dtos.Connector
{
    public record ConnectorDto : BaseDto
    {
        [JsonProperty("ampsMaxCurrent")]
        public int AmpsMaxCurrent { get; set; }

        [JsonProperty("chargeStationId")]
        public int ChargeStationId { get; set; }
    }
}

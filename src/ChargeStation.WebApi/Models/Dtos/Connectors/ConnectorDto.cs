using ChargeStation.WebApi.Models.Dtos.ChargeStation;
using Newtonsoft.Json;

namespace ChargeStation.WebApi.Models.Dtos.Connector
{
    public record ConnectorDto : BaseDto
    {
        [JsonProperty("ampsMaxCurrent")]
        public int AmpsMaxCurrent { get; set; }

        [JsonProperty("chargeStationId")]
        public int ChargeStationId { get; set; }

        [JsonProperty("chargeStation")]
        public ChargeStationDto ChargeStation { get; set; }
    }
}

using Newtonsoft.Json;

namespace ChargeStation.WebApi.Models.Dtos.Connector
{
    public record CreateUpdateConnectorResponseDto : BaseResponse
    {
        [JsonProperty("group")]
        public ConnectorDto Connector { get; set; }
    }
}

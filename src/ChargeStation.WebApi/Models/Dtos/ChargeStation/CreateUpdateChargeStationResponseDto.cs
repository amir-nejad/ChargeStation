using Newtonsoft.Json;

namespace ChargeStation.WebApi.Models.Dtos.ChargeStation
{
    public record CreateUpdateChargeStationResponseDto : BaseResponse
    {
        [JsonProperty("chargeStation")]
        public ChargeStationDto ChargeStation { get; set; }
    }
}

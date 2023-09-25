using Newtonsoft.Json;

namespace ChargeStation.WebApi.Models.Dtos
{
    public record BaseResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}

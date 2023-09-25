using Newtonsoft.Json;
using System;

namespace ChargeStation.WebApi.Models.Dtos
{
    public record BaseDto
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("createdDateUtc")]
        public DateTime? CreatedDateUtc { get; set; }

        [JsonProperty("lastModifiedUtc")]
        public DateTime? LastModifiedDateUtc { get; set; }
    }
}

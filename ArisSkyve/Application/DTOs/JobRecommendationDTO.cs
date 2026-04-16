using System.Text.Json.Serialization;

namespace ArisSkyve.Application.DTOs
{
    public class JobRecommendationDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("job")]
        public string JobTitle { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}

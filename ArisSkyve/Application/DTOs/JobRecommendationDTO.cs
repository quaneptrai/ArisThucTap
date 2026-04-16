using System.Text.Json.Serialization;

namespace ArisSkyve.Application.DTOs
{
    public class JobRecommendationDTO
    {
        public int Id { get; set; }

        [JsonPropertyName("job_title")]
        public string JobTitle { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }   

        [JsonPropertyName("final_score")]
        public double Score { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("advice")]
        public string Advice { get; set; }
    }
}

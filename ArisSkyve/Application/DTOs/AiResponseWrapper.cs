using System.Text.Json.Serialization;

namespace ArisSkyve.Application.DTOs
{
    public class AiResponseWrapper
    {
        [JsonPropertyName("recommendations")]
        public List<JobRecommendationDTO> Recommendations { get; set; }
    }
}

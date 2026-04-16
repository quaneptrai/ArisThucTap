using ArisSkyve.Domain.Entities;

namespace ArisSkyve.Application.DTOs
{
    public class JobsIndexViewModel
    {
        public User User { get; set; } = default!;
        public EmployesAccount Profile { get; set; } = default!;
        public List<JobPosting> JobPostings { get; set; } = new();
        public List<JobRecommendationDTO> RecommendedJobs { get; set; } = new();
    }
}

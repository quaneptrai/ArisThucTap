namespace ArisSkyve.Application.DTOs
{
    public class NewUserInforDTO
    {
        public int UserId { get; set; }
        // Step 1
        public string Location { get; set; }

        // Step 2
        public EducationDTO? Education { get; set; }
        public string? JobTitle { get; set; }
        public bool IsStudent { get; set; }

        // Step 3
        public string JobSearchingStatus { get; set; }
        public bool IsRemoteJob { get; set; }

        // Step 4
        public string DesiredJobTitle { get; set; }
        public string DesiredCompany { get; set; }

        // Step 5
        public bool ShareWithRecruiters { get; set; }
    }
}

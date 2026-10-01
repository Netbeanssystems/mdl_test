namespace Domain.Models
{
    public class ProjectResponse: BaseModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Yard { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public decimal OccupiedQuota { get; set; }
        public int TotalQuota { get; set; }
    }

    public class Project1Response : BaseModel
    {
        public int Id { get; set; }

        public decimal OccupiedQuota { get; set; }
        public int TotalQuota { get; set; }
    }
}

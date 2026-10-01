namespace Domain.Models
{
    public class BidderProjects: BaseModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int TotalQuota { get; set; }
        public decimal OccupiedQuota { get; set; }
    }
}

namespace Domain.Models
{
    public class BidderYards
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string YardNumber { get; set; } = string.Empty;
    }
}

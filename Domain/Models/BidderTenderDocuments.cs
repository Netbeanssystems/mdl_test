namespace Domain.Models
{
    public class BidderTenderDocuments : BaseModel
    {
        public int Id { get; set; }
        public string TenderNo { get; set; }
        public string DocTitle { get; set; }
        public string Doc { get; set; }
        public string DocType { get; set; }
        public string Remarks { get; set; }
        public string ProjectId { get; set; }
    }
}
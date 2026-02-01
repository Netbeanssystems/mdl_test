using System;

namespace Domain.Models
{
    public class BidderTenderUploads : BaseModel
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string YardId { get; set; }
        public string TenderNo { get; set; }
        public string TenderDescription { get; set; }
        public string ForeignBidderId { get; set; }
        public DateTime TenderOpeningDate { get; set; }
        public DateTime TenderClosingDate { get; set; }
        public string TenderDoc { get; set; }
    }
    public class BidderTenderCorrigendum : BaseModel
    {
        public int Id { get; set; }
        public string TenderId { get; set; }
        public string CorrigendumDescription { get; set; }
        public string CorrigendumDoc { get; set; }
        public DateTime? ExtendedDate { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class BidderTenderCorrigendumUploads : BaseModel
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string YardId { get; set; }
        public string TenderNo { get; set; }
        public string TenderDescription { get; set; }
        public string ForeignBidderId { get; set; }
        public DateTime? TenderStartDate { get; set; }
        public DateTime TenderOpeningDate { get; set; }
        public DateTime TenderClosingDate { get; set; }
        public string TenderDoc { get; set; }
        public List<BidderTenderCorrigendum> TenderCorrigendums { get; set; }
        public List<BidderTenderUploadDocuments> TenderDocuments { get; set; } = new();
    }
}

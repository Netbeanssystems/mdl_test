using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Application.Dtos
{
    public class BidderTenderUploadsDTO : BaseDTO
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public List<string> ProjectIds { get; set; } = new();
        public string YardId { get; set; }
        public List<string> YardIds { get; set; } = new();
        public string TenderNo { get; set; }
        public string TenderDescription { get; set; }
        public string ForeignBidderId { get; set; }
        public List<string> ForeignBidderIds { get; set; } = new();
        public DateTime? TenderStartDate { get; set; }
        public DateTime TenderOpeningDate { get; set; }
        public DateTime TenderClosingDate { get; set; }
        public string TenderDoc { get; set; }
        public IFormFile IFFTenderDoc { get; set; }
        public List<BidderTenderUploadDocumentsDTO> TenderDocuments { get; set; } = new();
        public List<string> UploadDocNames { get; set; } = new();
        public List<IFormFile> UploadDocFiles { get; set; } = new();
        public string DeletedDocIds { get; set; }
        public BidderTenderCorrigendumDto TenderCorrigendums { get; set; }
    }

    public class BidderTenderCorrigendumDto : BaseDTO
    {
        public int Id { get; set; }
        public string TenderId { get; set; }
        public string CorrigendumDescription { get; set; }
        public string CorrigendumDoc { get; set; }
        public IFormFile IFFCorrigendumDoc { get; set; }
        public DateTime? ExtendedDate { get; set; }
    }
}

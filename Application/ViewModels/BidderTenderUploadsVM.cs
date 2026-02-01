using Application.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Application.ViewModels
{
    public class BidderTenderUploadsVM : BaseVM
    {
        public int Id { get; set; }
        public string ProjectId { get; set; }
        public string EncryptedId { get; set; }
        public string ProjectName { get; set; }
        public string YardId { get; set; }
        public string TenderNo { get; set; }
        public string TenderDescription { get; set; }
        public string ForeignBidderId { get; set; }
        public DateTime TenderOpeningDate { get; set; }
        public DateTime TenderClosingDate { get; set; }
        public string TenderDoc { get; set; }
        public string TenderDocDecrypted{ get; set; }
        public DateTime? LatestExtendedDate { get; set; }
        public List<BidderTenderCorrigendumVM> TenderCorrigendums { get; set; }
    }
    public class BidderTenderCorrigendumVM : BaseVM
    {
        public int Id { get; set; }
        public string TenderId { get; set; }
        public string CorrigendumDescription { get; set; }
        public string CorrigendumDoc { get; set; }
        public string CorrigendumDocDecrypted { get; set; }
        public IFormFile IFFCorrigendumDoc { get; set; }
        public DateTime? ExtendedDate { get; set; }
    }
}

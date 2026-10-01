using System;

namespace Application.ViewModels
{
    public class BidderTenderDocumentsVM : BaseVM
    {
        public int Id { get; set; }
        public string TenderNo { get; set; }
        public string EncryptedId { get; set; }
        public string DocTitle { get; set; }
        public string ProjectId { get; set; }
        public string Doc { get; set; }
        public string DocType { get; set; }
        public string Remarks { get; set; }
        public string TenderDocDecrypted { get; set; }
        public DateTime? TenderOpeningDate { get; set; }
        public bool IsOpen { get; set; }
    }
}


using System;

namespace Domain.Models
{
    public class BidderTenderUploadDocuments
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public string OriginalFileName { get; set; }
        public string EncryptedFileName { get; set; }
        public long? FileSizeInBytes { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
    }
}

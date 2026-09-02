using System;

namespace Application.Dtos
{
    public class BidderTenderUploadDocumentsDTO
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

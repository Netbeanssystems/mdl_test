namespace Domain.Models
{
    public class BidderTenderUploadDocuments : BaseModel
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public string OriginalFileName { get; set; }
        public string EncryptedFileName { get; set; }
        public long? FileSizeInBytes { get; set; }
    }
}

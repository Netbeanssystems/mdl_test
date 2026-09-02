namespace Application.Dtos
{
    public class BidderTenderUploadDocumentsDTO : BaseDTO
    {
        public int Id { get; set; }
        public int TenderId { get; set; }
        public string OriginalFileName { get; set; }
        public string EncryptedFileName { get; set; }
        public long? FileSizeInBytes { get; set; }
    }
}

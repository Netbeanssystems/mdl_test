namespace Application.ViewModels
{
    public class GenaralUploadDocumentsVM : BaseVM
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentNameDecrypted { get; set; }
        public string Description { get; set; }
    }
}

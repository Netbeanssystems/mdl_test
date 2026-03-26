namespace Application.ViewModels
{
    public class DocumentsVM : BaseVM
    {
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentNameDecrypted { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string Description { get; set; }
        public int? URLsTimingId { get; set; }
        public URLsTimingVM URLsTiming { get; set; }
        public int? DocumentCount { get; set; }
    }
}

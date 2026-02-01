namespace Application.ViewModels
{
    public class DocumentsVM : BaseVM
    {
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentNameDecrypted { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }

    }
}

namespace Domain.Models
{
    public class Documents : BaseModel
    {
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public int? URLsTimingId { get; set; }
        public virtual URLsTiming URLsTiming { get; set; }
    }
}

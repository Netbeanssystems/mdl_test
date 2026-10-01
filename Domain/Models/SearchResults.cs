namespace Domain.Models
{
    public class SearchResults
    {
        public int Id { get; set; }
        public int? MenuId { get; set; }
        public int? MenuParentId { get; set; }
        public string Particulars { get; set; }
        public string Link { get; set; }
        public string Content { get; set; }
    }
}

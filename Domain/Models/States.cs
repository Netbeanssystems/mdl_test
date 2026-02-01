namespace Domain.Models
{
    public class States : BaseModel
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public virtual Countries Country { get; set; }
    }
}

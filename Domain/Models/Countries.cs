namespace Domain.Models
{
    public class Countries : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code3 { get; set; }
        public string Code2 { get; set; }
        public string Capital { get; set; }
        public string CurrencyCode { get; set; }
        //public virtual List<States> States { get; set; }
    }
}

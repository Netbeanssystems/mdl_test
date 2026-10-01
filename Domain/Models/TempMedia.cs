namespace Domain.Models
{
    public class TempMedia : BaseEntity
    {
        public int Id { get; set; }
        public string EnglishHeading { get; set; }
        public string HindiHeading { get; set; }
        public string EnglishContent { get; set; }
        public string HindiContent { get; set; }
        public string EnglishImage { get; set; }
        public string HindiImage { get; set; }
        public string EnglishFile { get; set; }
        public string HindiFile { get; set; }
        public bool Show { get; set; }
        public string Tag { get; set; }
        public string HindiTag { get; set; }
    }
}
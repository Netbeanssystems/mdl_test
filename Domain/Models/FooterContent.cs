using System;

namespace Domain.Models
{
    public class FooterContent
    {
        public int Id { get; set; }
        public string EnglishTitle { get; set; }
        public string EnglishPageLink { get; set; }
        public string HindiTitle { get; set; }
        public string HindiPageLink { get; set; }
        public int ColumnNo { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

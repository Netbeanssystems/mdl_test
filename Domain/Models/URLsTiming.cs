using System;

namespace Domain.Models
{
    public class URLsTiming : BaseModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }
}

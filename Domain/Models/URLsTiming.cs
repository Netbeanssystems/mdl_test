using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class URLsTiming : BaseModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string Description { get; set; }
        public bool IsShow { get; set; }
        public virtual ICollection<Documents> Documents { get; set; }
    }
}

using System;

namespace Domain.Models
{
    public class GeneraluploadURL : BaseModel
    {
        public int Id { get; set; }
        public string UploadUrl { get; set; }
        public string Description { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }
}

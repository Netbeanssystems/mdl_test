using System;
namespace Domain.Models
{
    public class BaseModel
    {
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string IP { get; set; }
    }
}

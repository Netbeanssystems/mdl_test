using System.Collections.Generic;
namespace Domain.Models
{
    public class Notifications : BaseModel
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public string Title { get; set; }
        public virtual List<NotificationDetails> NotificationDetails { get; set; }
    }
}

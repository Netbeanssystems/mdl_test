using System;

namespace Domain.Models
{
    public class LoginLogs
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
}
using System;

namespace Domain.Models
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Pwd1 { get; set; }
        public string Pwd2 { get; set; }
        public string Pwd3 { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

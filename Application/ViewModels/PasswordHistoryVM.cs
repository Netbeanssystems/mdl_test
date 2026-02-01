using System;

namespace Application.ViewModels
{
    public class PasswordHistoryVM
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Pwd1 { get; set; }
        public string Pwd2 { get; set; }
        public string Pwd3 { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}

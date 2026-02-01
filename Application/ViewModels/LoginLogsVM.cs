using System;

namespace Application.ViewModels
{
    public class LoginLogsVM
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
}
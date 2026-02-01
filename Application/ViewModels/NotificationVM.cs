using System.Collections.Generic;

namespace Application.ViewModels
{
    public class NotificationVM
    {
        public string IconClass { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Role { get; set; }
        public List<string> UserNames { get; set; }
        public string TargetUrl { get; set; }
        public string UserName { get; set; }
        public string Ip { get; set; }
    }
}

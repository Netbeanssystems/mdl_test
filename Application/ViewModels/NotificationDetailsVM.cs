namespace Application.ViewModels
{
    public class NotificationDetailsVM : BaseVM
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public string Text { get; set; }
        public string TargetUrl { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }

        public string Icon { get; set; }
        public string Title { get; set; }
    }
}

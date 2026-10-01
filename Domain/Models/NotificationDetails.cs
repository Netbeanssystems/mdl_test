namespace Domain.Models
{
    public class NotificationDetails : BaseModel
    {
        public int Id { get; set; }
        public int NotificationId { get; set; }
        public string Text { get; set; }
        public string TargetUrl { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public virtual Notifications Notification { get; set; }
    }
}

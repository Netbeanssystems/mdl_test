using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class NotificationDetailsDTO : BaseDTO
    {
        public int Id { get; set; }

        public int NotificationId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(256, ErrorMessage = "{1} characters max")]
        public string Text { get; set; }

        [DisplayName("Target Url")]
        [StringLength(128, ErrorMessage = "{1} characters max")]
        public string TargetUrl { get; set; }

        public string RoleName { get; set; }

        public string UserName { get; set; }

        public NotificationsDTO Notification { get; set; }
    }
}

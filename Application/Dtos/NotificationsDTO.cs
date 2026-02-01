using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class NotificationsDTO : BaseDTO
    {
        public int Id { get; set; }

        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string Icon { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string Title { get; set; }

        public List<NotificationDetailsDTO> NotificationDetails { get; set; }
    }
}

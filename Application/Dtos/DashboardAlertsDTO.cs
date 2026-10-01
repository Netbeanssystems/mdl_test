using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class DashboardAlertsDTO : BaseDTO
    {
        public int Id { get; set; }

        [DisplayName("Background Color")]
        //[Required(ErrorMessage = "{0} is required")]
        [StringLength(16, ErrorMessage = "{1} characters max")]
        public string BackgroundColor { get; set; }

        [DisplayName("Heading")]
        //[Required(ErrorMessage = "{0} is required")]
        [StringLength(512, ErrorMessage = "{1} characters max")]
        public string Heading { get; set; }

        public bool ShowHeading { get; set; }

        public bool BlinkHeading { get; set; }

        [DisplayName("Message Text")]
        //[Required(ErrorMessage = "{0} is required")]
        [StringLength(512, ErrorMessage = "{1} characters max")]
        public string Message { get; set; }

        [DisplayName("Start Date")]
        [Required(ErrorMessage = "{0} is required")]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [Required(ErrorMessage = "{0} is required")]
        public DateTime EndDate { get; set; }
    }
}

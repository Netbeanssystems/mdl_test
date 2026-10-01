using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class URLsTimingDTO : BaseDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Url { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string FromTimeStr { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string ToTimeStr { get; set; }
        
        // For service layer usage
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(256, ErrorMessage = "{1} characters max")]
        public string Description { get; set; }
    }


    public class ClosedWindowsDTO
    {
        public int Id { get; set; }

        public bool IsShow { get; set; }
    }
}

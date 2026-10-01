using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class GeneraluploadURLDTO : BaseDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string UploadUrl { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public DateTime FromTime { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public DateTime ToTime { get; set; }
    }
}

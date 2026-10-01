using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Dtos
{
    public class EventDTO : BaseDTO
    {
        public int Id { get; set; }

        [DisplayName("English Event Name")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(150, ErrorMessage = "{1} characters max")]
        public string EventName { get; set; }

        [DisplayName("Hindi Event Name")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(150, ErrorMessage = "{1} characters max")]
        public string EventNameHindi { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Column(TypeName = "date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime EventDate { get; set; }

        [DisplayName("English Event Description")]
        [Required(ErrorMessage = "{0} is required")]
        public string EnglishEventDescp { get; set; }

        [DisplayName("Hindi Event Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string HindiEventDescp { get; set; }
        [DisplayName("Hindi Event Name")]
        [Required(ErrorMessage = "{0} is required")]
        public string EventCategory { get; set; }
    }
}

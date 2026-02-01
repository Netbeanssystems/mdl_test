using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class BidderTenderDocumentsDTO: BaseDTO
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string TenderNo { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string DocTitle { get; set; }
        public string Doc { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string DocType { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Remarks { get; set; }
        public string ProjectId { get; set; }
    }
}

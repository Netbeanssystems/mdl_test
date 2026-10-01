using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class AcademicYearsDTO : BaseDTO
    {
        public int Id { get; set; }

        [DisplayName("Year")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(9, ErrorMessage = "{1} characters max")]
        public string Year { get; set; }

        [DisplayName("Description")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Description { get; set; }

        [DisplayName("Start Date")]
        [Required(ErrorMessage = "{0} is required")]
        public DateTime StartDate { get; set; }

        [DisplayName("End Date")]
        [Required(ErrorMessage = "{0} is required")]
        public DateTime EndDate { get; set; }
        public bool FinancialYearActive { get; set; }
        public bool FundRequestAllowed { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class StatesDTO : BaseDTO
    {
        public int Id { get; set; }

        [DisplayName("Country")]
        [Required(ErrorMessage = "{0} is required")]
        public int CountryId { get; set; }

        [DisplayName("State Name")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string StateName { get; set; }

        [DisplayName("State Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(2, ErrorMessage = "{1} characters max")]
        public string StateCode { get; set; }

        //[ForeignKey("CountryId")]
        //[InverseProperty("States")]
        //public virtual CountriesDTO Country { get; set; }
        //[InverseProperty("State")]
        //public virtual List<DistrictsDTO> Districts { get; set; }
    }
}

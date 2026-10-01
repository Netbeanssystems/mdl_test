using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class CountriesDTO : BaseDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(3, ErrorMessage = "{1} characters max")]
        public string Code3 { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(2, ErrorMessage = "{1} characters max")]
        public string Code2 { get; set; }

        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string Capital { get; set; }

        [StringLength(3, ErrorMessage = "{1} characters max")]
        public string CurrencyCode { get; set; }

        //[InverseProperty("Country")]
        //public virtual List<StatesDTO> States { get; set; }
    }
}

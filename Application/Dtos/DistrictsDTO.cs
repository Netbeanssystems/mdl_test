using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class DistrictsDTO : BaseDTO
    {
        public int Id { get; set; }

        [DisplayName("State")]
        [Required(ErrorMessage = "{0} is required")]
        public int StateId { get; set; }

        [DisplayName("District Name")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string DistrictName { get; set; }

        [DisplayName("District Code")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(3, ErrorMessage = "{1} characters max")]
        public string DistrictCode { get; set; }

        //[ForeignKey("StateId")]
        //[InverseProperty("Districts")]
        //public virtual StatesDTO State { get; set; }
        //[InverseProperty("District")]
        //public List<ITAssetsDTO> ITAssets { get; set; }
        //[InverseProperty("District")]
        //public List<ConstituenciesDTO> Constituencies { get; set; }
        //[InverseProperty("District")]
        //public List<SkillParksDTO> SkillParks { get; set; }
        //[InverseProperty("District")]
        //public List<SdcClassRoomsDTO> SdcClassRooms { get; set; }
    }
}

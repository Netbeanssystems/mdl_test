using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class MenuDTO : BaseDTO
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int Sequence { get; set; }

        [DisplayName("Menu Text")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string MenuText { get; set; }

        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string MenuTextHindi { get; set; }

        //[StringLength(64, ErrorMessage = "{1} characters max")]
        //public string MenuTextHindi { get; set; }

        [DisplayName("Icon Class")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string IconClass { get; set; }

        [DisplayName("Page Url")]
        [StringLength(128, ErrorMessage = "{1} characters max")]
        public string PageUrl { get; set; }

        public List<MenuDTO> Children { get; set; }
    }
}

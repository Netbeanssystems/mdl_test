using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class RoleMenusDTO
    {
        public int Id { get; set; }

        [DisplayName("Role Name")]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string RoleName { get; set; }

        [DisplayName("Menu")]
        [Required(ErrorMessage = "{0} is required")]
        public int MenuId { get; set; }
    }
}

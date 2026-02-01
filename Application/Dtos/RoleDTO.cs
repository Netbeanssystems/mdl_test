using System.ComponentModel.DataAnnotations;
namespace Application.Dtos
{
    public class RoleDTO
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Name { get; set; }
        public bool CanCreate { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}

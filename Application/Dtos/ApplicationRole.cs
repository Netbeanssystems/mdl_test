using Microsoft.AspNetCore.Identity;
namespace Application.Dtos
{
    public class ApplicationRole : IdentityRole
    {
        public bool CanCreate { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}

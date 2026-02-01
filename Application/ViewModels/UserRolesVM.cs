using Application.Dtos;
using System.Collections.Generic;
namespace Application.ViewModels
{
    public class UserRolesVM
    {
        public string userId { get; set; }
        public ApplicationUser user { get; set; }
        public string userRole { get; set; }
        public IList<string> userRoles { get; set; }
        public List<string> remainingRoles { get; set; }
    }
}
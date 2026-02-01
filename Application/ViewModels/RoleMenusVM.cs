using System.Collections.Generic;
namespace Application.ViewModels
{
    public class RoleMenusVM
    {
        public string roleName { get; set; }
        public int menuId { get; set; }
        public List<DropdownVM> assignedMenus { get; set; }
        public List<DropdownVM> remainingMenus { get; set; }
    }
}
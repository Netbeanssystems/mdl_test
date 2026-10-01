namespace Domain.Models
{
    public class RoleMenus
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public int MenuId { get; set; }
        public virtual Menus Menu { get; set; }
    }
}

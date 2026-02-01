using System.Collections.Generic;
namespace Domain.Models
{
    public class Menus : BaseModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int Sequence { get; set; }
        public string MenuText { get; set; }
        public string MenuTextHindi { get; set; }
        public string IconClass { get; set; }
        public string PageUrl { get; set; }
        public virtual Menus Parent { get; set; }
        public virtual List<Menus> Children { get; set; }
        public virtual List<RoleMenus> RoleMenus { get; set; }
    }
}
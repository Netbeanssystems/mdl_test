using System.Collections.Generic;
namespace Application.ViewModels
{
    public class MenusVM: BaseVM
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public int Sequence { get; set; }
        public string MenuText { get; set; }
        public string MenuTextHindi { get; set; }
        public string IconClass { get; set; }
        public string PageUrl { get; set; }
        public List<MenusVM> Children { get; set; }
    }
}

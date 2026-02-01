using System.Collections.Generic;

namespace Application.ViewModels
{
    public class OrgChartVM
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public virtual List<OrgChartVM> children { get; set; }
    }
}

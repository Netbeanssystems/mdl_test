using System;
using System.Collections.Generic;

namespace Application.Dtos.Datatables
{
    public class DatatableRequest
    {
        public string draw { get; set; }
        public int? start { get; set; }
        public int? length { get; set; }
        public Dictionary<string, string> searchColumns { get; set; }
        public string searchValue { get; set; }
        public string sortColumn { get; set; }
        public string sortDirection { get; set; }
        public int skip => start != null ? Convert.ToInt32(start) : 0;
        public int pageSize => length != null ? Convert.ToInt32(length) : 30;
        //Variables required for filtering on server
        public int? intFlag1 { get; set; }
        public int? intFlag2 { get; set; }
        public int? intFlag3 { get; set; }
        //public string strFlag1 {get; set;}
        //public string strFlag2 {get; set;}
    }
}

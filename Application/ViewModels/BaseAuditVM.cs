using System;

namespace Application.ViewModels
{
    public class BaseAuditVM
    {
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }
        public string IP { get; set; }
        public int? RowId { get; set; }
    }
}

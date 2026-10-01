using System;
namespace Domain.Models
{
    public class AcademicYears : BaseModel
    {
        public int Id { get; set; }
        public string Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public bool FinancialYearActive { get; set; }
        public bool FundRequestAllowed { get; set; }
    }
}

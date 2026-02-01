using System;
namespace Application.ViewModels
{
    public class AcademicYearsVM : BaseVM
    {
        public int Id { get; set; }
        public string Year { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool FinancialYearActive { get; set; }
        public bool FundRequestAllowed { get; set; }
        public bool? IsSelect { get; set; }
    }
}

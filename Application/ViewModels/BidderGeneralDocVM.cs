using System;
namespace Application.ViewModels
{
    public class BidderGeneralDocVM : BaseVM
    {
        public int Id { get; set; }

        public int TotalQuota { get; set; }
        public decimal OccupiedQuota { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string IP { get; set; }

    }
}

using System;
namespace Domain.Models
{
    public class BidderGeneralDoc : BaseModel
    {
        public int Id { get; set; }

        public int TotalQuota { get; set; }
        public decimal OccupiedQuota { get; set; }

    }
}

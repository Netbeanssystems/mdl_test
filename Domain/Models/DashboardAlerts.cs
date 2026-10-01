using System;
namespace Domain.Models
{
    public class DashboardAlerts : BaseModel
    {
        public int Id { get; set; }
        public string BackgroundColor { get; set; }
        public string Heading { get; set; }
        public bool ShowHeading { get; set; }
        public bool BlinkHeading { get; set; }
        public string Message { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

using System;

namespace Domain.Models
{
    public class ContactUsForm
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string CountryName { get; set; }
        public string InterestedIn { get; set; }
        public string EmailId { get; set; }
        public string ToEmailId { get; set; }
        public string Contact { get; set; }
        public string Message { get; set; }
        public DateTime SubmitOn { get; set; }
        public string IP { get; set; }
        public string SubmitedFrom { get; set; }
    }
}

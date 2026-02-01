using System;

namespace Domain.Models
{
    public class EnquiryForm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Organization { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public int Fax { get; set; }
        public string PostalAddress { get; set; }
        public string ComplaintDetails { get; set; }
        public int? ShipBuildRelated { get; set; }
        public int? SubHeavyRelated { get; set; }
        public DateTime SubmitOn { get; set; }
        public string IP { get; set; }
    }
}

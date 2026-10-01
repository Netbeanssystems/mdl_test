using System;

namespace Application.ViewModels
{
    public class GrievanceVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Organization { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Fax { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public int CountryId { get; set; }
        public string Address { get; set; }
        public string Grievance { get; set; }
        public DateTime SubmitOn { get; set; }
        public string IP { get; set; }
    }
}

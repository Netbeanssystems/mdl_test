using System;

namespace Application.ViewModels
{
    public class FeedbackVM
    {
        public int Id { get; set; }
        public int RatingNo1 { get; set; }
        public int RatingNo2 { get; set; }
        public int RatingNo3 { get; set; }
        public int RatingNo4 { get; set; }
        public int RatingNo5 { get; set; }
        public int RatingNo6 { get; set; }
        public int RatingNo7 { get; set; }
        public int RatingNo8 { get; set; }
        public string ReasonSatisfied { get; set; }
        public string ReasonDisSatisfied { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Organization { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public DateTime SubmitOn { get; set; }
        public string IP { get; set; }
    }
}

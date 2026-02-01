using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    // For Executives only
    public class CareerDTO
    {
        public string DatePosting { get; set; }
        public string AdvRefNo { get; set; }
        public string Post { get; set; }
        public string Details { get; set; }
        public string Link { get; set; }
        public string AdvRefNumberHindi { get; set; } //Akash
        public string PostHindi { get; set; }//akash
        public string DetailsHindi { get; set; }//akash
        public string FileName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Added_on { get; set; }
        public string ClosingDate { get; set; }
    }

    // For Non-Executives and Apprentice only
    public class NonCareerDTO
    {
        public string DatePosting { get; set; }
        public string AdvRefNo { get; set; }
        public string AdvRefNumberHindi { get; set; } //Akash
        public string Link { get; set; }
        public string FileName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Added_on { get; set; }
        public string ClosingDate { get; set; }
    }

    // For Retired Executive and Non-Executive
    public class RetireCareerDTO
    {
        public string DatePosting { get; set; }
        public string Notification { get; set; }
        public string NotificationHindi { get; set; }
        public string Link { get; set; }
        public string FileName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Added_on { get; set; }
        public string ClosingDate { get; set; }
    }
}

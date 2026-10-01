using System;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels
{
    public class PhotoGalleryVM
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public bool Active { get; set; }
        public string EnglishHeading { get; set; }
        public string HindiHeading { get; set; }
        public string ShowEnglishImage { get; set; }
        public string ShowHindiImage { get; set; }
        public string EnglishDescp { get; set; }
        public string HindiDescp { get; set; }
        public int? Priority { get; set; }
        public string EnglishAttachment { get; set; }
        public string HindiAttachment { get; set; }
        public DateTime CreateDate { get; set; }
        public string EventName { get; set; }
    }

    public class TopEventsVM
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventNameHindi { get; set; }
        public string EnglishAttachment { get; set; }
        public string HindiAttachment { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Added_on { get; set; }
        public string EventCategory { get; set; }
    }

    public class EventPhotosVM
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventNameHindi { get; set; }
        public string EnglishAttachment { get; set; }
        public string HindiAttachment { get; set; }
        public string EnglishHeading { get; set; }
        public string HindiHeading { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime Added_on { get; set; }
    }
}

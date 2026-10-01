using System;

namespace Application.ViewModels
{
    public class EventVM : BaseVM
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string EventNameHindi { get; set; }
        public DateTime EventDate { get; set; }
        public string EnglishEventDescp { get; set; }
        public string HindiEventDescp { get; set; }
        public string EventCategory { get; set; }

    }
}

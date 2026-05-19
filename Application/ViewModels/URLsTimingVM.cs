using System;

namespace Application.ViewModels
{
    public class URLsTimingVM : BaseVM
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string Description { get; set; }
        public virtual string URLsTimingId { get; set; }
    }

    public class ClosedWindowsVM
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsShow { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }
}

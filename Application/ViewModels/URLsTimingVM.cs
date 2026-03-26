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
    }
}

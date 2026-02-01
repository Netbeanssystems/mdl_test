using System;

namespace Application.ViewModels
{
    public class GeneraluploadURLVM : BaseVM
    {
        public int Id { get; set; }
        public string UploadUrl { get; set; }
        public string Description { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }
}

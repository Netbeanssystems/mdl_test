using Microsoft.AspNetCore.Http;
using System;

namespace Application.ViewModels
{
    public class TempPhotoGalleryVM : BaseAuditVM
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public bool Active { get; set; }
        public string EnglishHeading { get; set; }
        public string HindiHeading { get; set; }
        public DateTime CreateDate { get; set; }
        public string ShowEnglishImage { get; set; }
        public string ShowHindiImage { get; set; }
        public string EnglishDescp { get; set; }
        public string HindiDescp { get; set; }
        public int? Priorty { get; set; }
        public string EnglishText { get; set; }
        public string HindiText { get; set; }
        public string EnglishAttachment { get; set; }
        public IFormFile EnglishAttachmentfile { get; set; }
        public string HindiAttachment { get; set; }
        public IFormFile HindiAttachmentfile { get; set; }


    }
}

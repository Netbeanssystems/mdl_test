using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class PhotoGalleryDTO
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public bool Active { get; set; }

        [Required(ErrorMessage = "English Heading required !")]
        public string EnglishHeading { get; set; }

        [Required(ErrorMessage = "Hindi Heading required !")]
        public string HindiHeading { get; set; }
        public string ShowEnglishImage { get; set; }
        public string ShowHindiImage { get; set; }
        public string EnglishDescp { get; set; }
        public string HindiDescp { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }
        public int? Priority { get; set; }
        public string EnglishAttachment { get; set; }
        public string HindiAttachment { get; set; }

        public IFormFile EnglishAttachmentfile { get; set; }
        public IFormFile HindiAttachmentfile { get; set; }
        public string nextVal { get; set; }
        public int PageNo { get; set; }
        public string Keyesdata { get; set; }
        //public string HindiHeadingenc => HindiHeading.Replace("\r\n", "");
    }
}

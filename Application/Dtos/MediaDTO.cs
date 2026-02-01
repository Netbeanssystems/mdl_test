using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class MediaDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Heading required !")]
        public string EnglishHeading { get; set; }
        [Required(ErrorMessage = "Heading required !")]
        public string HindiHeading { get; set; }
        public string EnglishContent { get; set; }
        public string HindiContent { get; set; }
        public string EnglishImage { get; set; }
        public string HindiImage { get; set; }
        public string EnglishFile { get; set; }
        public string HindiFile { get; set; }


        [Required(ErrorMessage = "NewsDate required !")]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[DataType(DataType.Date)]
        public DateTime? UploadedDate { get; set; }
        public bool Show { get; set; }
        public IFormFile FileEnglish { get; set; }
        public IFormFile FileHindi { get; set; }
        public string Tag { get; set; }
        public string HindiTag { get; set; }
        public string nextVal { get; set; }
        public int PageNo { get; set; }
        public string Minpage { get; set; }
    }
}

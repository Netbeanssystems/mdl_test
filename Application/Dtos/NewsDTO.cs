using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class NewsDTO
    {
        public int Id { get; set; }
        public long? LoginUserId { get; set; }
        public long? AcceptedBy { get; set; }
        [Required(ErrorMessage = "Heading required !")]

        public string EnglishHeadingName { get; set; }
        //[Url]
        [Display(Name = "English Page Link")]
        public string EnglishPageLink { get; set; }
        public string EnglishContentDesc { get; set; }
        public string EnglishAttachment { get; set; }
        public int? ParentId { get; set; }
        [Required(ErrorMessage = "Heading required !")]
        public string HindiHeadingName { get; set; }
        //[Url]
        [Display(Name = "Hindi Page Link")]
        public string HindiPageLink { get; set; }
        public string HindiContentDesc { get; set; }
        public string HindiAttachment { get; set; }
        public DateTime? ValidTill { get; set; }
        public long? ForReview { get; set; }
        public int? Priority { get; set; }
        public int? Status { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string IsSubHeading { get; set; }
        public bool Show { get; set; }
        //  [FileFormate(RestrictExtention: "exe")]
        public IFormFile EnglishFile { get; set; }
        //  [FileFormate(RestrictExtention: "exe")]
        public IFormFile HindiFile { get; set; }

        public IFormFile EnglishImage { get; set; }
        //  [FileFormate(RestrictExtention: "exe")]
        public IFormFile HindiImage { get; set; }
    }
}

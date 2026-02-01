using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels
{
    public class WhatsNewVM
    {
        public int Id { get; set; }
        public long? LoginUserId { get; set; }
        public long? AcceptedBy { get; set; }
        [Required(ErrorMessage = "Heading required !")]
        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string EnglishHeadingName { get; set; }
        public string EnglishPageLink { get; set; }
        public string EnglishContentDesc { get; set; }
        public string EnglishAttachment { get; set; }
        public int? ParentId { get; set; }
        [Required(ErrorMessage = "Heading required !")]
        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string HindiHeadingName { get; set; }
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
        public virtual ICollection<WhatsNewVM> Children { get; set; }
        public DateTime? PublishDate { get; set; }
    }


    //Show = x.Show,
    //             Priority   = x.Priority,
    //             EnglishAttachment = x.EnglishAttachment,
    //             PublishDate = x.PublishDate,
    //             EnglishContentDesc = x.EnglishContentDesc,
    //             EnglishPageLink = x.EnglishPageLink,
    //             HindiContentDesc = x.HindiContentDesc,
    public class WhatsNewModal
    {
        public string EnglishPageLink { get; set; }
        public string EnglishContentDesc { get; set; }
        public string EnglishAttachment { get; set; }
        public string HindiContentDesc { get; set; }
        public int? Priority { get; set; }
        public bool Show { get; set; }
        public DateTime? PublishDate { get; set; }
    }


}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels
{
    public class MenuHeadingsVM
    {
        public int Id { get; set; }
        public long? LoginUserId { get; set; }
        public long? AcceptedBy { get; set; }
        [Required(ErrorMessage = "Heading required !")]
        //[StringLength(32, ErrorMessage = "{1} characters max")]
        public string EnglishHeadingName { get; set; }
        public string EnglishPageLink { get; set; }
        public string EnglishContentDesc { get; set; }
        public string EnglishAttachment { get; set; }
        public int? ParentId { get; set; }
        [Required(ErrorMessage = "Heading required !")]
        //[StringLength(32, ErrorMessage = "{1} characters max")]
        public string HindiHeadingName { get; set; }
        public string URLHeadingEnglish { get; set; }
        public string URLHeadingHindi { get; set; }
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
        public string Title { get; set; }
        public string HindiTitle { get; set; }
        public string Description { get; set; }
        public string Keyword { get; set; }
        public string KeywordHindi { get; set; }
        public string DescriptionHindi { get; set; }
        public virtual ICollection<MenuHeadingsVM> Children { get; set; }
        public bool Clickable { get; set; }
    }

    public class MenuHeadingsCustomVM
    {
        public int Id { get; set; }
        public string EnglishHeadingName { get; set; }
        //public string URLHeadingEnglish { get; set; }
        //public string URLHeadingHindi { get; set; }
        public string EnglishPageLink { get; set; }
        public string HindiContentDesc { get; set; }
        public string EnglishContentDesc { get; set; }
        public string HindiHeadingName { get; set; }
        public string HindiPageLink { get; set; }
        //public string HindiContentDesc { get; set; }
        //public string HindiAttachment { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keyword { get; set; }
        //public string HindiTitle { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? Priority { get; set; }
        public int? ParentId { get; set; }
        public bool Show { get; set; }
        public bool Clickable { get; set; }
        public virtual ICollection<MenuHeadingsCustomVM> Children { get; set; }
    }
    public class MenuHeadingsInternalVM
    {
        public int Id { get; set; }
        public string EnglishHeadingName { get; set; }
        public string EnglishPageLink { get; set; }
        public string EnglishContentDesc { get; set; }
        public string HindiHeadingName { get; set; }
        public string HindiPageLink { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keyword { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? Priority { get; set; }
        public int? ParentId { get; set; }
        public bool Show { get; set; }
        public bool Clickable { get; set; }

    }





    public class MenuHeadingsCustomVMModal
    {
        public int Id { get; set; }
        string EnglishHeadingName { get; set; }
        public string EnglishPageLink { get; set; }
        public string EnglishContentDesc { get; set; }
        public string HindiHeadingName { get; set; }
        public int? Priority { get; set; }
        public bool Show { get; set; }
        public bool Clickable { get; set; }
        public virtual ICollection<MenuHeadingsCustomVM> Children { get; set; }
    }
}

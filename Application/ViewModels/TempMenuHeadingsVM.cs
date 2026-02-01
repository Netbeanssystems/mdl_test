using System;
using System.Collections.Generic;

namespace Application.ViewModels
{
    public class TempMenuHeadingsVM : BaseAuditVM
    {
        public int Id { get; set; }
        public long? LoginUserId { get; set; }
        public long? AcceptedBy { get; set; }
        public string EnglishHeadingName { get; set; }
        public string URLHeadingEnglish { get; set; }
        public string URLHeadingHindi { get; set; }
        public string EnglishPageLink { get; set; }
        public string EnglishContentDesc { get; set; }
        public string EnglishAttachment { get; set; }
        public int? ParentId { get; set; }
        public string HindiHeadingName { get; set; }
        public string HindiPageLink { get; set; }
        public string HindiContentDesc { get; set; }
        public string HindiAttachment { get; set; }
        public DateTime? ValidTill { get; set; }
        public long? ForReview { get; set; }
        public int? Priority { get; set; }
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
    }
}
using System;

namespace Domain.Models
{
    public class News
    {
        public int Id { get; set; }
        public long? LoginUserId { get; set; }
        public long? AcceptedBy { get; set; }
        public string EnglishHeadingName { get; set; }
        public int LanguageId { get; set; }
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
        public int Priority { get; set; }
        public int Status { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string IsSubHeading { get; set; }
        public bool Show { get; set; }
        public virtual News Parent { get; set; }
    }

    //Show = x.Show,
    //            EnglishContentDesc = x.EnglishContentDesc,
    //            Priority = x.Priority,
    //            HindiContentDesc = x.HindiContentDesc,
    public class NewsData
    {
        public int Id { get; set; }
        public string EnglishContentDesc { get; set; }
        public string HindiContentDesc { get; set; }
        public int Priority { get; set; }
        public bool Show { get; set; }
        public virtual NewsData Parent { get; set; }
    }
}

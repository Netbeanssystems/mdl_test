using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class TempBanners : BaseEntity
    {
        public int Id { get; set; }
        public string Linkopen { get; set; }

        [Required(ErrorMessage = "Heading required !")]
        public string EnglishHeadingName { get; set; }

        [Required(ErrorMessage = "Heading required !")]
        public string HindiHeadingName { get; set; }
        public string EnglishBanner { get; set; }
        public string HindiBanner { get; set; }
        public string EnglishLink { get; set; }
        public string HindiLink { get; set; }
        public int? ParentId { get; set; }
        public long? LoginUserId { get; set; }
        public long? AcceptedBy { get; set; }
        public DateTime? ValidTill { get; set; }
        public long? ForReview { get; set; }
        public int? Priority { get; set; }
        public DateTime? ArchiveDate { get; set; }
        public DateTime? SubmitDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string IsSubHeading { get; set; }
        public bool Show { get; set; }
        public virtual Banners Parent { get; set; }
    }
}
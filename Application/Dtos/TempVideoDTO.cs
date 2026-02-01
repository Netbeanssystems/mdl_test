namespace Application.Dtos
{
    public class TempVideoDTO : BaseAuditDTO
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string EnglishHeading { get; set; }
        public string HindiHeading { get; set; }
        public string EnglishThumbnailLink { get; set; }
        public string HindThumbnailLink { get; set; }
        public string EnglishLink { get; set; }
        public string HindiLink { get; set; }
        public string EnglishCategory { get; set; }
        public string HindiCategory { get; set; }

    }
}

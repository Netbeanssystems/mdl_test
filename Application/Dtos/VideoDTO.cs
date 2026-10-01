using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class VideoDTO
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        [Required(ErrorMessage = "English Heading required !")]
        public string EnglishHeading { get; set; }
        public string HindiHeading { get; set; }
        [Required(ErrorMessage = "English Link required !")]
        public string EnglishLink { get; set; }
        [Required(ErrorMessage = "Hindi Link required !")]
        public string HindiLink { get; set; }
        public string EnglishCategory { get; set; }
        public string HindiCategory { get; set; }
        [Required(ErrorMessage = "English Thumbnail Link required !")]
        public string EnglishThumbnailLink { get; set; }
        [Required(ErrorMessage = "Hind Thumbnail Link required !")]
        public string HindThumbnailLink { get; set; }
    }
}

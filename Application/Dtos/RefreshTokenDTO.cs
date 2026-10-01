using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Application.Dtos
{
    public class RefreshTokenDTO
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Token { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime IssuedUtc { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm tt}", ApplyFormatInEditMode = false)]
        public DateTime ExpiresUtc { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(10, ErrorMessage = "{1} characters max")]
        public string UserId { get; set; }
    }
}

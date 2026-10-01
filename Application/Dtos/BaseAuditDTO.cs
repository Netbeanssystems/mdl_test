using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Dtos
{
    public class BaseAuditDTO
    {
        [Required(ErrorMessage = "*required")]
        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string Action { get; set; }

        [Required(ErrorMessage = "*required")]
        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}", ApplyFormatInEditMode = false)]
        public DateTime ActionDate { get; set; }

        [Required(ErrorMessage = "*required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "*required")]
        [StringLength(32, ErrorMessage = "{1} characters max")]
        public string RoleName { get; set; }

        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string Role { get; set; }
        [Required(ErrorMessage = "*required")]
        [StringLength(16, ErrorMessage = "{1} characters max")]
        public string Status { get; set; }

        [Required(ErrorMessage = "*required")]
        [StringLength(16, ErrorMessage = "{1} characters max")]
        public string IP { get; set; }

        public int? RowId { get; set; }
    }
}

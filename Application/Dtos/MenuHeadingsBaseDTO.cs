using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Dtos
{
    public class MenuHeadingsBaseDTO
    {
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "*required")]
        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}", ApplyFormatInEditMode = false)]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "*required")]
        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}", ApplyFormatInEditMode = false)]
        public DateTime? ModifiedDate { get; set; }

        [StringLength(64, ErrorMessage = "{1} characters max")]
        public string ModifiedBy { get; set; }

        [Required(ErrorMessage = "*required")]
        [StringLength(16, ErrorMessage = "{1} characters max")]
        public string IP { get; set; }
    }
}

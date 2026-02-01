using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class BidderProjectsDTO : BaseDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s&,\-/]+$",ErrorMessage = "Project Name can only contain letters, numbers, spaces, and the characters - , & /.")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Yard From must be a number.")]
        public string YardFrom { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Yard To must be a number.")]
        [YardValidation]
        public string YardTo { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-&,/]+$", ErrorMessage = "Remarks can only contain letters, numbers, spaces, and the characters - , & /.")]
        public string Remarks { get; set; } = string.Empty;

        public int TotalQuota { get; set; }
        public decimal OccupiedQuota { get; set; }
    }

    public class UpdateQuotaDTO
    {
        public int Id { get; set; }
        public decimal OccupiedQuota { get; set; }
    }

    public class BidderYardsDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string YardNumber { get; set; } = string.Empty;
    }

    public class YardValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (BidderProjectsDTO)validationContext.ObjectInstance;
            if (int.TryParse(model.YardFrom, out int from) && int.TryParse(model.YardTo, out int to))
            {
                if (to <= from)
                {
                    return new ValidationResult("Yard To must be greater than Yard From.");
                }
            }
            return ValidationResult.Success;
        }
    }
}

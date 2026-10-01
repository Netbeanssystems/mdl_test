namespace Application.Dtos
{
    public class SendSmsNotificationsDTO : BaseDTO
    {
        public int Id { get; set; }
        //  public int StudentId { get; set; }
        public int InstitutionId { get; set; }
        //[StringLength(10, ErrorMessage = "{1} characters max")]
        //[RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10 digit number")]
        //public string MobileNo { get; set; }
        //[Required(ErrorMessage = "{0} is required")]
        //[StringLength(64, ErrorMessage = "{1} characters max")]
        //[RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9-]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Enter a valid email id")]
        //public string EmailId { get; set; }
        //[Required(ErrorMessage = "{0} is required")]
        //[StringLength(32, ErrorMessage = "{1} characters max")]
        public string Notification { get; set; }
    }
}

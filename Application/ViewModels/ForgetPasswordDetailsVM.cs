using System;

namespace Application.ViewModels
{
    public class ForgetPasswordDetailsVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

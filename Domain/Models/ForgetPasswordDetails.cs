using System;

namespace Domain.Models
{
    public class ForgetPasswordDetails
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

using System;

namespace Application.Dtos
{
    public class ForgetPasswordDetailsDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

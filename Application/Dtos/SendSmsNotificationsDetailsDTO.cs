using System.Collections.Generic;

namespace Application.Dtos
{
    public class SendSmsNotificationsDetailsDTO : BaseDTO
    {
        public List<string> MobileNos { get; set; }
        public string Message { get; set; }
    }
}

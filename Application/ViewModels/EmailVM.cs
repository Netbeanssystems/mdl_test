using System.Collections.Generic;
using System.Net.Mail;

namespace Application.ViewModels
{
    public class EmailVM
    {
        public List<string> ToAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; } = true;
        public List<string> CcAddresses { get; set; }
        public List<string> BccAddresses { get; set; }
        public List<Attachment> Attachments { get; set; }
        public List<SendGrid.Helpers.Mail.Attachment> SendGridAttachments { get; set; }
    }
}

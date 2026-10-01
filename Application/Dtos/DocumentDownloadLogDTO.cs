using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class DocumentDownloadLogDTO
    {
        public string DocumentName { get; set; }
        public string DownloadedBy { get; set; }
        public DateTimeOffset DownloadedAt { get; set; }
        public string IpAddress { get; set; }
    }
}

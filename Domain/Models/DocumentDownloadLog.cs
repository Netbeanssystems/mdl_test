using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class DocumentDownloadLog
    {
        public long Id { get; set; }
        public string DocumentName { get; set; }
        public string DownloadedBy { get; set; }
        public DateTimeOffset DownloadedAt { get; set; }
        public string IpAddress { get; set; }
    }
}

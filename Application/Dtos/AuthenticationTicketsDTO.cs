using System;
namespace Application.Dtos
{
    public class AuthenticationTicketsDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public byte[] Value { get; set; }
        public DateTimeOffset? LastActivity { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public string RemoteIpAddress { get; set; }
        public string OperatingSystem { get; set; }
        public string UserAgentFamily { get; set; }
        public string UserAgentVersion { get; set; }
    }
}

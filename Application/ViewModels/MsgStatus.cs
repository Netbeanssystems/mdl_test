using Newtonsoft.Json;

namespace Application.ViewModels
{
    public class MsgStatus
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
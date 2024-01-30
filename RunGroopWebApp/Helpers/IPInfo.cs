using Newtonsoft.Json;

namespace RunGroopWebApp.Helpers
{
    public class IPInfo
    {
        // Hit API endpoint to fetch data
        // IPInfo object will translate the JSON returned

        [JsonProperty("ip")]
        public string Ip { get; set; }
        [JsonProperty("hostname")]
        public string Hostname { get; set; }
        public string City { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("loc")]
        public string Location { get; set; }
        [JsonProperty("org")]
        public string Org { get; set; }
        [JsonProperty("postal")]
        public string Postal { get; set; }
        [JsonProperty("timezone")]
        public string Timezone { get; set; }

    }
}

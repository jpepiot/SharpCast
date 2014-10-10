namespace SharpCast {
    using Newtonsoft.Json;

    public class Application {
        [JsonProperty("appId")]
        public string ApplicationId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("transportId")]
        public string TransportId { get; set; }

        [JsonProperty("statusText")]
        public string StatusText { get; set; }
    }
}

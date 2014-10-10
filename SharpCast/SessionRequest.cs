namespace SharpCast {
    using Newtonsoft.Json;

    public abstract class SessionRequest : Request {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }
    }
}

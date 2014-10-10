namespace SharpCast {
    using Newtonsoft.Json;

    public abstract class Message {
        [JsonProperty("type")]
        public abstract string Type { get; }
    }
}

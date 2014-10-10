namespace SharpCast {
    using Newtonsoft.Json;

    public abstract class Request : Message {
        [JsonProperty("requestId")]
        public int RequestId { get; set; }
    }
}

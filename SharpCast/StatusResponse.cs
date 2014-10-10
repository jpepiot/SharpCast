namespace SharpCast {
    using Newtonsoft.Json;

    public class StatusResponse : Response {
        [JsonProperty("status")]
        public Status Status { get; set; }

        public override string Type {
            get { return "RECEIVER_STATUS"; }
        }
    }
}

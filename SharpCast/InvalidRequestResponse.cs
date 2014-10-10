namespace SharpCast {
    using Newtonsoft.Json;

    public class InvalidRequestResponse : Response {

        [JsonProperty("reason")]
        public string Reason { get; set; }

        public override string Type {
            get { return "INVALID_REQUEST"; }
        }
    }
}

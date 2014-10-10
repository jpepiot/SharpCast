namespace SharpCast {
    using Newtonsoft.Json;

    public class LoadFailedResponse : Response {

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }    

        public override string Type {
            get { return "LOAD_FAILED"; }
        }
    }
}

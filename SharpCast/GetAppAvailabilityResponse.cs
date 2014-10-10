namespace SharpCast {
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class GetAppAvailabilityResponse : Response {

        [JsonProperty("availability")]
        public Dictionary<string, string> Availability { get; set; }

        public override string Type {
            get { return "GET_APP_AVAILABILITY"; }
        }
    }
}

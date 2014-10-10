namespace SharpCast {
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Status {
        [JsonProperty("applications")]
        public List<Application> Applications { get; set; }

        [JsonProperty("isActiveInput")]
        public bool IsActiveInput { get; set; }

        [JsonProperty("volume")]
        public Volume Volume { get; set; }
    }
}

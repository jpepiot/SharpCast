namespace SharpCast {
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Media {

        [JsonProperty("contentId")]
        public string ContentId { get; set; }

        [JsonProperty("streamType")]
        public StreamType StreamType { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("metadata")]
        public MediaMetadata Metadata { get; set; }

        [JsonProperty("duration")]
        public double? Duration { get; set; }

        [JsonProperty("customData")]
        public Dictionary<string, string> CustomData { get; set; }
    }
}

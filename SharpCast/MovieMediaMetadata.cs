namespace SharpCast {
    using Newtonsoft.Json;

    public class MovieMediaMetadata : MediaMetadata {

        public override int MetadataType {
            get { return 2; }
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subtitle")]
        public string SubTitle { get; set; }

        [JsonProperty("studio")]
        public string Studio { get; set; }

        [JsonProperty("images")]
        public Image[] Images { get; set; }
    }
}

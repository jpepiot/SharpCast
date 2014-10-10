namespace SharpCast {
    using Newtonsoft.Json;

    public class GenericMediaMetadata : MediaMetadata {

        public override int MetadataType {
            get { return 0; }
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subtitle")]
        public string SubTitle { get; set; }

        [JsonProperty("images")]
        public Image[] Images { get; set; }
    }
}

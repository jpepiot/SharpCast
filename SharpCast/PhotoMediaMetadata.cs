namespace SharpCast {
    using Newtonsoft.Json;

    public class PhotoMediaMetadata : MediaMetadata {

        public override int MetadataType {
            get { return 4; }
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }
    }
}

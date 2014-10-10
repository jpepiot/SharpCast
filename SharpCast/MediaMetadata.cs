namespace SharpCast {
    using Newtonsoft.Json;

    public abstract class MediaMetadata {
        [JsonProperty("metadataType")]
        public abstract int MetadataType { get; }
    }
}

namespace SharpCast {
    using Newtonsoft.Json;

    public class MusicTrackMediaMetadata : MediaMetadata {

        public override int MetadataType {
            get { return 3; }
        }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("albumName")]
        public string AlbumName { get; set; }

        [JsonProperty("albumArtist")]
        public string AlbumArtist { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("trackNumber")]
        public int? TrackNumber { get; set; }

        [JsonProperty("discNumber")]
        public int? DiscNumber { get; set; }

        [JsonProperty("images")]
        public Image[] Images { get; set; }
    }
}

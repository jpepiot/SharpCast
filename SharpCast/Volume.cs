namespace SharpCast {
    using Newtonsoft.Json;

    public class Volume {

        public Volume(double? level, bool muted) {
            Level = level;
            Muted = muted;
        }

        [JsonProperty("level")]
        public double? Level { get; set; }

        [JsonProperty("muted")]
        public bool Muted { get; set; }
    }
}

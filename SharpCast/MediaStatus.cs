namespace SharpCast {
    using Newtonsoft.Json;

    public class MediaStatus {
        [JsonProperty("mediaSessionId")]
        public long MediaSessionId { get; set; }

        [JsonProperty("media")]
        public Media Media { get; set; }

        [JsonProperty("playbackRate")]
        public int PlaybackRate { get; set; }

        [JsonProperty("playerState")]
        public PlayerStateType PlayerState { get; set; }

        [JsonProperty("currentTime")]
        public float CurrentTime { get; set; }

        [JsonProperty("volume")]
        public Volume Volume { get; set; }

        [JsonProperty("supportedMediaCommands")]
        public int SupportedMediaCommands { get; set; }

        [JsonProperty("idleReason")]
        public string IdleReason { get; set; }
    }
}

namespace SharpCast {
    using Newtonsoft.Json;

    public class PlayRequest : SessionRequest {

        private readonly long _mediaSessionId;

        public PlayRequest(string sessionId, long mediaSessionId) {
            SessionId = sessionId;
            _mediaSessionId = mediaSessionId;
        }

        public override string Type {
            get {
                return "PLAY";
            }
        }

        [JsonProperty("mediaSessionId")]
        public long MediaSessionId {
            get {
                return _mediaSessionId;
            }
        }
    }
}

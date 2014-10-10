namespace SharpCast {
    using Newtonsoft.Json;

    public class PauseRequest : SessionRequest {

        private readonly long _mediaSessionId;

        public PauseRequest(string sessionId, long mediaSessionId) {
            SessionId = sessionId;
            _mediaSessionId = mediaSessionId;
        }

        public override string Type {
            get {
                return "PAUSE";
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

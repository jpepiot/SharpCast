namespace SharpCast {
    using Newtonsoft.Json;

    public class SeekRequest : SessionRequest {

        private readonly long _mediaSessionId;
        private readonly double _currentTime;

        public SeekRequest(string sessionId, long mediaSessionId, double currentTime) {
            SessionId = sessionId;
            _mediaSessionId = mediaSessionId;
            _currentTime = currentTime;
        }

        public override string Type {
            get {
                return "SEEK";
            }
        }

        [JsonProperty("mediaSessionId")]
        public long MediaSessionId {
            get {
                return _mediaSessionId;
            }
        }

        [JsonProperty("currentTime")]
        public double CurrentTime {
            get {
                return _currentTime;
            }
        }
    }
}

namespace SharpCast {
    using Newtonsoft.Json;

    public class LoadRequest : SessionRequest {

        private readonly Media _media;
        private readonly bool _autoplay;

        public LoadRequest(string sessionId, Media media, bool autoplay) {
            SessionId = sessionId;
            _media = media;
            _autoplay = autoplay;
        }

        public override string Type {
            get {
                return "LOAD";
            }
        }

        [JsonProperty("media")]
        public Media Media {
            get {
                return _media;
            }
        }

        [JsonProperty("autoplay")]
        public bool AutoPlay {
            get {
                return _autoplay;
            }
        }
    }
}

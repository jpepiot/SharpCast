namespace SharpCast {
    using Newtonsoft.Json;

    public class SetVolumeRequest : SessionRequest {

        private readonly Volume _volume;

        public SetVolumeRequest(Volume volume) {
            _volume = volume;
        }

        public override string Type {
            get {
                return "SET_VOLUME";
            }
        }

        [JsonProperty("volume")]
        public Volume Volume {
            get {
                return _volume;
            }
        }
    }
}

namespace SharpCast {
    using Newtonsoft.Json;

    public class LaunchRequest : Request {

        private readonly string _appId;

        public LaunchRequest(string appId) {
            _appId = appId;
        }

        public override string Type {
            get {
                return "LAUNCH";
            }
        }

        [JsonProperty("appId")]
        public string AppId {
            get {
                return _appId;
            }
        }
    }
}

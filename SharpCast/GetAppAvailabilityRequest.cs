namespace SharpCast{
    using Newtonsoft.Json;

    public class GetAppAvailabilityRequest : Request {

        private readonly string[] _appIds;

        public GetAppAvailabilityRequest(string[] appIds) {
            _appIds = appIds;
        }

        public override string Type {
            get {
                return "GET_APP_AVAILABILITY";
            }
        }

        [JsonProperty("appId")]
        public string[] AppIds {
            get {
                return _appIds;
            }
        }
    }
}

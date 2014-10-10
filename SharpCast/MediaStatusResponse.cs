namespace SharpCast {
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class MediaStatusResponse : Response {
        [JsonProperty("status")]
        public List<MediaStatus> Statuses { get; set; }

        public override string Type {
            get { return "MEDIA_STATUS"; }
        }
    }
}

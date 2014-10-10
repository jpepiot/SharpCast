namespace SharpCast {
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MediaMetadataJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return typeof(MediaMetadata).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var jsonObject = JObject.Load(reader);
            int type = int.Parse(jsonObject["metadataType"].ToString());
            MediaMetadata mediaMetadata = null;
            switch (type) {
                case 1:
                    mediaMetadata = new GenericMediaMetadata();
                    break;
                case 2:
                    mediaMetadata = new MovieMediaMetadata();
                    break;
                case 3:
                    mediaMetadata = new MusicTrackMediaMetadata();
                    break;
                case 4:
                    mediaMetadata = new PhotoMediaMetadata();
                    break;
            }

            if (mediaMetadata != null) {
                serializer.Populate(jsonObject.CreateReader(), mediaMetadata);
            }

            return mediaMetadata;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new System.NotImplementedException();
        }
    }
}

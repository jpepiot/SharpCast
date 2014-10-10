namespace SharpCast {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Newtonsoft.Json;

    public abstract class Response : Message {

        private static readonly Dictionary<string, Type> ResponseTypes = new Dictionary<string, Type>();

        static Response() {
            Assembly ass = Assembly.GetExecutingAssembly();
            foreach (var type in ass.GetTypes().Where(x => typeof(Response).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)) {
                ResponseTypes.Add(((Response)Activator.CreateInstance(type)).Type, type);
            }
        }

        [JsonProperty("requestId")]
        public int RequestId { get; set; }

        public static Type GetResponseType(string type) {
            if (ResponseTypes.ContainsKey(type)) {
                return ResponseTypes[type];
            }

            return null;
        }
    }
}

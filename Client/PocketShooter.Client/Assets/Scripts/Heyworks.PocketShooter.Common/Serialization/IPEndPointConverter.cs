using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Heyworks.PocketShooter.Meta.Serialization
{
    public class IPEndPointConverter : JsonConverter<IPEndPoint>
    {
        public override void WriteJson(JsonWriter writer, IPEndPoint value, JsonSerializer serializer)
        {
            new JObject
                {
                    {nameof(IPEndPoint.Address), JToken.FromObject(value.Address, serializer)},
                    {nameof(IPEndPoint.Port), value.Port},
                }
                .WriteTo(writer);
        }

        public override IPEndPoint ReadJson(JsonReader reader, Type objectType, IPEndPoint existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject json = JObject.Load(reader);
            IPAddress address = json[nameof(IPEndPoint.Address)].ToObject<IPAddress>(serializer);
            var port = (int)json[nameof(IPEndPoint.Port)];
            return new IPEndPoint(address, port);
        }
    }
}

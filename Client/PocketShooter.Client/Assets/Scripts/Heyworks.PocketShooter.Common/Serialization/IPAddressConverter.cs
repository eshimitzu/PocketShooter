using Newtonsoft.Json;
using System;
using System.Net;

namespace Heyworks.PocketShooter.Meta.Serialization
{
    public class IPAddressConverter : JsonConverter<IPAddress>
    {
        public override void WriteJson(JsonWriter writer, IPAddress value, JsonSerializer serializer) =>
            writer.WriteValue(value.ToString());

        public override IPAddress ReadJson(JsonReader reader, Type objectType, IPAddress existingValue, bool hasExistingValue, JsonSerializer serializer) =>
            IPAddress.Parse((string)reader.Value);
    }
}

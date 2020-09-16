using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Heyworks.PocketShooter.Meta.Serialization;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Utils.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Heyworks.PocketShooter.Modules.GameEnvironment
{
    [ExecuteInEditMode]
    public class ZoneConfig : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> dominationZones = null;

        [SerializeField]
        private GameObject prefab = null;

        [SerializeField]
        private float radius = 2f;

#if UNITY_EDITOR
        [ContextMenu("Print zones")]
        private void PrintZones()
        {
            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.CurrentCulture = culture;

            var sb = new StringBuilder();
            sb.AppendLine("        private static readonly ZoneConfig[] xxxZoneConfigs =");
            sb.AppendLine("        {");

            int id = 0;
            foreach (Transform t in dominationZones)
            {
                sb.AppendLine($"            new ZoneConfig({id++}, {t.position.x}f, {t.position.y}f, {t.position.z}f, {radius}f),");
            }

            sb.AppendLine("        };");
            Debug.Log(sb);
        }

        private void OnDrawGizmos()
        {
            if (prefab && !Application.isPlaying)
            {
                foreach (Transform t in dominationZones)
                {
                    prefab.Render(
                        transform.position + t.localPosition,
                        Quaternion.identity,
                        Vector3.one * ZoneView.ScaleForOneMeter * radius);
                }
            }
        }

        [ContextMenu("Update Config")]
        public void UpdateConfig()
        {
            string filePath = Application.dataPath;
            string clientPath = Path.Combine("Client", "PocketShooter.Client", "Assets");
            string serverPath = Path.Combine("Server", ".gameconfigs", "pocketshooter.json");
            filePath = filePath.Replace(clientPath, serverPath);

            JObject data = null;
            using (StreamReader file = File.OpenText(filePath))
            {
                var settings = new DefaultSerializerSettings();
                settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                JsonSerializer serializer = JsonSerializer.Create(settings);
                data = (JObject)serializer.Deserialize(file, typeof(object));

                if (data["RealtimeConfig"]["DominationModeConfig"]["MapConfig"]["ZonesConfig"] is JArray zones)
                {
                    zones.Clear();

                    byte index = 0;
                    foreach (Transform t in dominationZones)
                    {
                        Vector3 pos = t.position;
                        var info = new DominationZoneInfo(index++, pos.x, pos.y, pos.z, radius);
                        JObject zone = JObject.FromObject(info, serializer);
                        zones.Add(zone);
                    }
                }
            }

            using (StreamWriter file = File.CreateText(filePath))
            using (var writer = new JsonTextWriter(file) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t' })
            {
                data.WriteTo(writer);
            }
        }
#endif
    }
}
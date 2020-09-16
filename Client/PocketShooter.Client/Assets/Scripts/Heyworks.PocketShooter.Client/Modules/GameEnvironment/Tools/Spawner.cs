using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Serialization;
using Heyworks.PocketShooter.Realtime.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Heyworks.PocketShooter.Modules.GameEnvironment
{
    /// <summary>
    /// Contains SpawnerTeamPoints for all game mode types.
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        [SerializeField]
        private List<SpawnerTeamPoints> spawnerTeamsPoints = null;

#if UNITY_EDITOR
        [ContextMenu("Print spawn points")]
        public void PrintSpawnPoints()
        {
            CultureInfo culture = CultureInfo.GetCultureInfo("en-US");
            CultureInfo.CurrentCulture = culture;

            var sb = new StringBuilder();
            sb.AppendLine("private static readonly TeamConfig[] xxxTeamConfigs =");
            sb.AppendLine("{");

            foreach (SpawnerTeamPoints t in spawnerTeamsPoints)
            {
                sb.AppendLine($"new TeamConfig(TeamNo.{t.Team.ToString()}, new[]");
                sb.AppendLine("{");
                foreach (Transform p in t.SpawnPoints)
                {
                    var pos = p.position;
                    sb.AppendLine(
                        $"new TeamConfig.SpawnPointConfig({pos.x}f, {pos.y}f, {pos.z}f, {p.rotation.eulerAngles.y}f),");
                }

                sb.AppendLine("}),");
            }

            sb.AppendLine("};");
            Debug.Log(sb);
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
            using (var reader = new JsonTextReader(file))
            {
                var settings = new DefaultSerializerSettings();
                settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                JsonSerializer serializer = JsonSerializer.Create(settings);
                data = (JObject)serializer.Deserialize(reader, typeof(object));

                if (data["RealtimeConfig"]["DominationModeConfig"]["MapConfig"]["TeamsConfig"] is JArray teams)
                {
                    teams.Clear();

                    foreach (SpawnerTeamPoints t in spawnerTeamsPoints)
                    {
                        var points = new List<SpawnPointInfo>();

                        foreach (Transform p in t.SpawnPoints)
                        {
                            Vector3 pos = p.position;
                            points.Add(new SpawnPointInfo(pos.x, pos.y, pos.z, p.rotation.eulerAngles.y));
                        }

                        var config = new TeamInfo(t.Team, points.ToArray());

                        JObject team = JObject.FromObject(config, serializer);
                        teams.Add(team);
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
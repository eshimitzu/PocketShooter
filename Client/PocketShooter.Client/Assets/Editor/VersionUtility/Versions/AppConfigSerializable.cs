using System;
using System.IO;
using System.Text;
using Heyworks.PocketShooter.Serialization;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility.Versions
{
    // TODO: a.dezhurko Delete. Use AppConfiguration with extension methods instead. 
    public class AppConfigSerializable
    {
        public string version;
        public string metaServerIp;
        public string metaServerPort;
        public string bundleId;

        public AppConfigSerializable(AppConfig config)
        {
            version = config.Version;
            metaServerIp = config.MetaServerIp;
            metaServerPort = config.MetaServerPort;
            bundleId = config.BundleId;
        }

        public void Save(string appConfigPath)
        {
            string filePath = GetFilePath(appConfigPath);
            var encoding = new UTF8Encoding();

            var serializer = new JSONSerializer();
            var serializedConfig = serializer.Serialize(this, Formatting.Indented);

            try
            {
                File.WriteAllText(filePath, serializedConfig, encoding);
            }
            catch (Exception e)
            {
                throw new Exception(message: $"Error writing configuration file to {filePath}.", e);
            }

            AssetDatabase.Refresh();
        }

        private static string GetFilePath(string relativePath)
        {
            try
            {
                return Application.dataPath + relativePath;
            }
            catch (Exception e)
            {
                throw new Exception(message: $"Error writing configuration file to {relativePath}.", e);
            }
        }
    }
}
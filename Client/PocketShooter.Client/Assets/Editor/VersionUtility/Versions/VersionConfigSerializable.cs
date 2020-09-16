using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility.Versions
{
    public class VersionConfigSerializable : SerializableConfiguration<VersionConfigSerializable>
    {
        public Version[] Versions
        {
            get;
            set;
        }

        public VersionConfigSerializable()
        {
            Versions = new Version [] {new Version()};
        }

        public VersionConfigSerializable(VersionName versionName, PlatformName[] platformNames)
        {
            var versions = new List<Version>();
            foreach (var platformName in platformNames)
            {
                versions.Add(new Version {VersionName = versionName, PlatformName = platformName });
            }

            Versions = versions.ToArray();
        }

        public Version GetVersion(VersionName versionName, PlatformName currentPlatformName)
        {
            foreach (Version version in Versions)
            {
                if (version.VersionName.Equals(versionName))
                {
                    if (version.PlatformName.Equals(currentPlatformName))
                    {
                        return version;
                    }
                }
            }

            return null;
        }

        public static VersionConfigSerializable GetFromFile(string relativePath)
        {
            VersionConfigSerializable versionsConfiguration = null;
            string path = GetFilePath(relativePath);

            if (File.Exists(path))
            {
                var configData = File.ReadAllText(path);
                versionsConfiguration = Deserialize(configData);
            }

            return versionsConfiguration;
        }

        public static string GetFilePath(string relativePath)
        {
            try
            {
                return Application.dataPath + relativePath;
            }
            catch (Exception e)
            {
                throw new Exception(message: $"Error writing version configuration file to {relativePath}.", e);
            }
        }

        public void Save(string relativePath)
        {
            string filePath = GetFilePath(relativePath);

            var encoding = new UTF8Encoding();

            try
            {
                var serializedConfig = SerializeWithIndentedFormatting();
                File.WriteAllText(filePath, serializedConfig, encoding);
            }
            catch (Exception e)
            {
                throw new Exception(message: $"Error writing configuration file to {filePath}.", e);
            }

            AssetDatabase.Refresh();
        }
    }
}
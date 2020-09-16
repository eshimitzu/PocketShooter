using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.Extensions.Logging
{
    /// <inheritdoc/>
    [System.Serializable]
    public class LoggerConfiguration : ILoggerConfiguration
    {
        [SerializeField]
        private List<LogFilterConfiguration> filters = null;
        private string fileName = "MLog";

        /// <summary>
        /// Gets the filters.
        /// </summary>
        public List<LogFilterConfiguration> Filters => filters;

        /// <inheritdoc/>
        public System.Action<ILoggerConfiguration> OnChanged { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerConfiguration"/> class.
        /// </summary>
        public LoggerConfiguration()
        {
            TextAsset file = Resources.Load<TextAsset>(fileName);
            if (file)
            {
                JsonUtility.FromJsonOverwrite(file.text, this);
            }
        }

        /// <inheritdoc/>
        public void Reload()
        {
            TextAsset file = Resources.Load<TextAsset>(fileName);
            if (file)
            {
                JsonUtility.FromJsonOverwrite(file.text, this);
            }

            OnChanged?.Invoke(this);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
#if UNITY_EDITOR
            TextAsset file = Resources.Load<TextAsset>(fileName);
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(file);
            string appPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            string filePath = appPath + assetPath;

            string json = JsonUtility.ToJson(this, true);
            System.IO.File.WriteAllText(filePath, json);

            UnityEditor.EditorUtility.SetDirty(file);
            UnityEditor.AssetDatabase.ImportAsset(assetPath);
#endif
        }
    }
}

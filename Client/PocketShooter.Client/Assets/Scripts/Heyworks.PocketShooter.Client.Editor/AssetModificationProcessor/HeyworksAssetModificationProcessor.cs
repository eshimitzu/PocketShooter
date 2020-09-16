using System;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Heyworks.PocketShooter.AssetModificationProcessor
{
    /// <summary>
    /// Represents the hook for file's generation from unity editor.
    /// Checkout unity3d c# script template for #NAMESPACE# keyword.
    /// </summary>
    /// <seealso cref="UnityEditor.AssetModificationProcessor" />
    public class HeyworksAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        private const string NamespaceTemplateKeyword = "#NAMESPACE#";
        private const string RootDirectoryKeyword = "Assets";
        private const string ScriptsDirectoryName = "Scripts";

        /// <summary>
        /// Called when the asset is created.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", string.Empty);
            int index = path.LastIndexOf(".", StringComparison.Ordinal);
            if (index < 0)
            {
                return;
            }

            string fileExtension = path.Substring(index);
            if (fileExtension != ".cs")
            {
                return;
            }

            index = Application.dataPath.LastIndexOf(RootDirectoryKeyword, StringComparison.Ordinal);
            var systemPath = Application.dataPath.Substring(0, index) + path;
            if (!File.Exists(systemPath))
            {
                return;
            }

            string fileContent = File.ReadAllText(systemPath);
            if (fileContent.Contains(NamespaceTemplateKeyword))
            {
                fileContent = fileContent.Replace(NamespaceTemplateKeyword, GetNamespaceForPath(path));

                File.WriteAllText(path, fileContent);
                AssetDatabase.Refresh();
            }
        }

        private static string GetNamespaceForPath(string path)
        {
            int lastDirectorySeparatorIndex = path.LastIndexOf(Path.AltDirectorySeparatorChar);
            path = path.Remove(lastDirectorySeparatorIndex);

            string[] directories = path.Split(new[] { Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            string resultNamespace = string.Empty;
            var scriptsDirectoryNameFound = false;

            foreach (var dir in directories)
            {
                if (scriptsDirectoryNameFound)
                {
                    resultNamespace += dir;
                    resultNamespace += '.';
                }
                else if (dir.Equals(ScriptsDirectoryName))
                {
                    scriptsDirectoryNameFound = true;
                }
            }

            resultNamespace = resultNamespace.TrimEnd('.');

            return scriptsDirectoryNameFound ? resultNamespace : EditorSettings.projectGenerationRootNamespace;
        }
    }
}
#endif

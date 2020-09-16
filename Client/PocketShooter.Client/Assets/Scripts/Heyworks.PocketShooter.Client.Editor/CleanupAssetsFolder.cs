using System.IO;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter
{
    /// <summary>
    /// Removes all garbage from the asset folder.
    /// </summary>
    public class CleanupAssetsFolder
    {
        /// <summary>
        /// Executes clean-up.
        /// </summary>
        [MenuItem("Tools/Clean up assets folder")]
        public static void Execute()
        {
            ProcessDirectory(Directory.GetCurrentDirectory() + @"/Assets");
            ProcessOrigs(Directory.GetCurrentDirectory() + @"/Assets");
            ProcessRejects(Directory.GetCurrentDirectory() + @"/Assets");
            AssetDatabase.Refresh();
        }

        private static void ProcessDirectory(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                ProcessDirectory(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory);
                    File.Delete(directory + ".meta");
                    Debug.Log(directory + " deleted");
                }
            }
        }

        private static void ProcessOrigs(string startLocation)
        {
            string[] files = Directory.GetFiles(startLocation, "*.orig", SearchOption.AllDirectories);
            foreach (var item in files)
            {
                Debug.Log(item + " deleted");
                File.Delete(item);
                if (File.Exists(item + ".meta"))
                {
                    File.Delete(item + ".meta");
                }
            }
        }

        private static void ProcessRejects(string startLocation)
        {
            string[] files = Directory.GetFiles(startLocation, "*.rej", SearchOption.AllDirectories);
            foreach (var item in files)
            {
                Debug.Log(item + " deleted");
                File.Delete(item);
                if (File.Exists(item + ".meta"))
                {
                    File.Delete(item + ".meta");
                }
            }
        }
    }
}

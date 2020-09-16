using Heyworks.PocketShooter.VersionUtility.Versions;
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility.Components
{
    public class ProjectSettingsImporterEditor : IEditorComponent
    {
        private readonly VersionManager versionManager;

        public ProjectSettingsImporterEditor(VersionManager versionManager)
        {
            this.versionManager = versionManager;
        }

        public void OnDisable()
        {

        }

        public void OnEnable()
        {

        }

        public void OnComponentGUI()
        {
            if (GUILayout.Button("WRITE SETTINGS TO EDITOR", GUILayout.Height(50)))
            {
                var version = versionManager.GetCurrentVersion();
                ProjectSettingsManager.WriteSettingsToEditor(version);
            }
        }

        public string GetComponentName()
        {
            return "MANAGE SETTINGS";
        }
    }
}
using Heyworks.PocketShooter.VersionUtility.Versions;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility.Components
{
    public class VersionSelectorComponentEditor : IEditorComponent
    {
        private readonly VersionManager versionManager;
        private VersionName currentVersionName;
        private PlatformName currentPlatformName;

        public VersionSelectorComponentEditor(VersionManager versionManager)
        {
            this.versionManager = versionManager;
            this.currentVersionName = versionManager.CurrentVersionName;
            this.currentPlatformName = versionManager.CurrentPlatformName;
        }

        public void OnDisable()
        {
        }

        public void OnEnable()
        {
        }

        public void OnComponentGUI()
        {
            if (versionManager != null)
            {
                GUILayout.Label(GetComponentName(), EditorStyles.boldLabel);
                ShowVersionsUI();
            }
        }

        public string GetComponentName()
        {
            return "VERSION SELECTOR";
        }

        private void ShowVersionsUI()
        {
            currentVersionName = (VersionName)EditorGUILayout.EnumPopup("Version", currentVersionName);
            currentPlatformName = (PlatformName)EditorGUILayout.EnumPopup("Platform", currentPlatformName);

            if (GUI.changed)
            {
                versionManager.SetCurrentVersion(currentVersionName, currentPlatformName);
            }
        }
    }
}
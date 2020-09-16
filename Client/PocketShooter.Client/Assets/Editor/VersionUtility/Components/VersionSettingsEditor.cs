using Heyworks.PocketShooter.VersionUtility.Versions;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility.Components
{
    public abstract class VersionSettingsEditor : IEditorComponent
    {
        private VersionObject currentVersionObject;

        protected VersionManager VersionManager { get; }

        protected SerializedObject SerializedObject { get; private set;  }


        protected VersionSettingsEditor(VersionManager versionManager)
        {
            this.VersionManager = versionManager;

            Initialize();
        }

        public abstract string GetComponentName();

        public abstract void OnComponentGUI();

        public void OnDisable()
        {
            VersionManager.CurrentVersionChanged -= VersionsConfiguration_CurrentVersionChanged;
        }

        public void OnEnable()
        {
            Initialize();
            VersionManager.CurrentVersionChanged += VersionsConfiguration_CurrentVersionChanged;
        }

        protected void Initialize()
        {
            Version currentVersion = VersionManager.GetCurrentVersion();

            if (currentVersion != null)
            {
                currentVersionObject = (VersionObject)ScriptableObject.CreateInstance(typeof(VersionObject));
                currentVersionObject.AndroidSettings = currentVersion.AndroidSettings;
                currentVersionObject.IOSSettings = currentVersion.IOSSettings;
                currentVersionObject.CommonSettings = currentVersion.CommonSettings;
                SerializedObject = new SerializedObject(currentVersionObject);
            }
        }

        private void VersionsConfiguration_CurrentVersionChanged()
        {
            var version = VersionManager.GetCurrentVersion();
            if (version != null)
            {
                currentVersionObject.AndroidSettings = version.AndroidSettings;
                currentVersionObject.IOSSettings = version.IOSSettings;
                currentVersionObject.CommonSettings = version.CommonSettings;
            }
        }
    }
}
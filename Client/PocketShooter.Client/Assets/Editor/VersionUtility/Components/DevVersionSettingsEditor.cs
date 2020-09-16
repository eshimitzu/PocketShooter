using Heyworks.PocketShooter.VersionUtility.Versions;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility.Components
{
    public class DevVersionSettingsEditor : VersionSettingsEditor
    {
        public DevVersionSettingsEditor(VersionManager versionManager)
            : base(versionManager)
        {

        }

        public override void OnComponentGUI()
        {
            if (VersionManager.CurrentVersionName == VersionName.Prod)
            {
                return;
            }

            GUILayout.Label(GetComponentName(), EditorStyles.boldLabel);
            var config = SerializedObject;

            if (config != null && config.targetObject != null)
            {
                GUI.changed = false;
                config.Update();
                SerializedProperty serializedProperty = config.GetIterator();
                if (serializedProperty != null)
                {
                    bool enterChildren = true;
                    while (serializedProperty.NextVisible(enterChildren))
                    {
                        enterChildren = false;
                        bool hide = false;
                        hide |= (serializedProperty.name == "m_Script");
                        hide |= (VersionManager.CurrentPlatformName != PlatformName.iOS && serializedProperty.name == "IOSSettings");
                        hide |= (VersionManager.CurrentPlatformName != PlatformName.Android && serializedProperty.name == "AndroidSettings");
                        if (!hide)
                        {
                            EditorGUILayout.PropertyField(serializedProperty, true);
                        }
                    }

                    config.ApplyModifiedProperties();
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("SAVE VERSIONS CONFIGURATION TO \n" + VersionManager.CurrentVersionConfigFilePath, GUILayout.Height(60)))
                {
                    VersionManager.SaveVersionConfiguration();
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("SAVE APP CONFIGURRATION TO \n" + VersionManager.GetAppConfigPath(), GUILayout.Height(60)))
                {
                    VersionManager.SaveAppConfiguration();
                }
            }
            else
            {
                Initialize();
            }
        }

        public override string GetComponentName()
        {
            return "PROJECT SETTINGS";
        }
    }
}
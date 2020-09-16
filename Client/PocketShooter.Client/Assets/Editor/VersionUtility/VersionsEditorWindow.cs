using Heyworks.PocketShooter.VersionUtility.Components;
using Heyworks.PocketShooter.VersionUtility.Versions;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility
{
    public class VersionsEditorWindow : EditorWindow
    {
        private static EditorWindow window;
        private IEditorComponent[] editorComponents;
        private Vector2 scrollViewPosition;
        private bool isEditorReady;

        public static void Init()
        {
            CloseWindow();
            window = GetWindow(typeof(VersionsEditorWindow), false, "Version settings");
            window.Show();
        }

        public static void CloseWindow()
        {
            if (window != null)
            {
                window.Close();
            }
        }

        protected void OnEnable()
        {
            isEditorReady = false;

            VersionManager versionManager = new VersionManager();

            editorComponents = new IEditorComponent[]
                           {
                               new VersionSelectorComponentEditor(versionManager),
                               new DevVersionSettingsEditor(versionManager),
                               new ProdVersionSettingsEditor(versionManager),
                               new ProjectSettingsImporterEditor(versionManager),
                           };

            foreach (var editorComponent in editorComponents)
            {
                editorComponent.OnEnable();
            }

            isEditorReady = true;
        }

        protected void OnDisable()
        {
            isEditorReady = false;
            if (editorComponents != null)
            {
                foreach (var editorComponent in editorComponents)
                {
                    if (editorComponent != null)
                    {
                        editorComponent.OnDisable();
                    }
                }
            }
        }

        public void OnGUI()
        {
            isEditorReady &= !EditorApplication.isCompiling;
            isEditorReady &= !EditorApplication.isPlayingOrWillChangePlaymode;
            isEditorReady &= !EditorApplication.isUpdating;
            isEditorReady &= (editorComponents != null && editorComponents.Length > 0);

            EditorGUILayout.BeginVertical();
            scrollViewPosition = EditorGUILayout.BeginScrollView(scrollViewPosition);

            if (isEditorReady)
            {
                foreach (var editorComponent in editorComponents)
                {
                    if (editorComponent != null)
                    {
                        EditorGUILayout.Space();
                        editorComponent.OnComponentGUI();
                        EditorGUILayout.Space();
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("UPDATING, PLEASE WAIT", MessageType.Warning, true);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}
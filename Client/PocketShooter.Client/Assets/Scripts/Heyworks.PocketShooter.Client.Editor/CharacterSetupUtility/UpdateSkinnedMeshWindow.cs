using System;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.CharacterSetupUtility
{
    public class UpdateSkinnedMeshWindow : EditorWindow
    {
        private static EditorWindow window;
        private SkinnedMeshRenderer targetSkin;
        private Transform sourceRootBone;
        private CharacterPrefabManager characterPrefabManager;

        public static void Init()
        {
            CloseWindow();
            window = GetWindow(typeof(UpdateSkinnedMeshWindow), false, "Update Skinned Mesh Bones");
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
            characterPrefabManager = new CharacterPrefabManager();
        }

        private void OnGUI()
        {
            targetSkin = EditorGUILayout.ObjectField("Target", targetSkin, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
            sourceRootBone = EditorGUILayout.ObjectField("SourceRootBone", sourceRootBone, typeof(Transform), true) as Transform;

            GUI.enabled = (targetSkin != null && sourceRootBone != null);

            if (GUILayout.Button("Update Skinned Mesh Renderer"))
            {
                characterPrefabManager.UpdateBones(targetSkin, sourceRootBone);
            }
        }
    }
}
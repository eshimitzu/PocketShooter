using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.CharacterSetupUtility
{
    public class CharacterSetupWindow : EditorWindow
    {
        private static EditorWindow window;
        private Vector2 scrollViewPosition;

        private bool isEditorReady;

        private CharacterPrefabManager characterPrefabManager;
        private TroopersConfigManager troopersConfigManager;
        private IconsFactoryManager iconsFactoryManager;
        private CharacterInfo characterInfo;
        private SerializedObject characterInfoSerializedObject;

        private string instructions;

        public static bool ShowWarningWithOkAndCancel(string warningMessage)
        {
            if (EditorUtility.DisplayDialog("Warning", warningMessage, "OK", "CANCEL"))
            {
                return true;
            }

            return false;
        }

        public static void Init()
        {
            CloseWindow();
            window = GetWindow(typeof(CharacterSetupWindow), false, "Create new character");
        }

        public static void CloseWindow()
        {
            if (window != null)
            {
                window.Close();
            }
        }

        public void OnGUI()
        {
            CheckIfEditorReady();

            if (!isEditorReady)
            {
                EditorGUILayout.HelpBox("UPDATING, PLEASE WAIT", MessageType.Warning, true);
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "To create new character, first add a new field to TrooperClass enum at \n " +
                "Assets/Scripts/Heyworks.PocketShooter.Common/TrooperClass.cs", MessageType.Info);

            GUI.changed = false;
            characterInfoSerializedObject.Update();
            SerializedProperty serializedProperty = characterInfoSerializedObject.GetIterator();

            EditorGUILayout.BeginVertical();
            scrollViewPosition = EditorGUILayout.BeginScrollView(scrollViewPosition);
            EditorGUILayout.Space();

            if (serializedProperty != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Common Settings", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                bool enterChildren = true;
                while (serializedProperty.NextVisible(enterChildren))
                {
                    bool hide = false;
                    hide |= (serializedProperty.name == "m_Script");
                    hide |= (characterInfo.CreateNewAnimatorOverrideController == true && serializedProperty.name == "animatorOverrideController");
                    hide |= (characterInfo.CreateNewAnimatorOverrideController == false && serializedProperty.name == "animationsFolder");
                    hide |= (characterInfo.CreateNewAnimatorOverrideController == false && serializedProperty.name == "saveAnimatorOverrideControllerTo");

                    if (serializedProperty.name == "createNewAnimatorOverrideController")
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Animator Settings", EditorStyles.boldLabel);
                        EditorGUILayout.Space();
                    }

                    if (!hide)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.PropertyField(serializedProperty, false);
                        EditorGUILayout.Space();
                    }
                }

                characterInfoSerializedObject.ApplyModifiedProperties();

                EditorGUILayout.Space();

                if (GUILayout.Button("CREATE NEW CHARACTER", GUILayout.Height(50)))
                {
                    try
                    {
                        ValidateCharacterInfoFields();

                        characterPrefabManager.CreateCharacter(characterInfo);

                        troopersConfigManager.AddCharacterToTroopersConfig(characterInfo);

                        iconsFactoryManager.AddCharacterToIconsFactory(characterInfo);
                    }
                    catch (Exception e)
                    {
                        ShowWarningWithOkAndCancel(e.Message);
                    }
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(instructions, MessageType.Warning, true);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        protected void OnEnable()
        {
            characterPrefabManager = new CharacterPrefabManager();
            troopersConfigManager = new TroopersConfigManager();
            iconsFactoryManager = new IconsFactoryManager();

            characterInfo = (CharacterInfo)ScriptableObject.CreateInstance(typeof(CharacterInfo));
            characterInfoSerializedObject = new SerializedObject(characterInfo);

            instructions = CreateInstructionsString();

            isEditorReady = true;
        }

        private void ValidateCharacterInfoFields()
        {
            bool isErrorDetected = false;

            StringBuilder errorStringBuilder = new StringBuilder(300);

            if (string.IsNullOrEmpty(characterInfo.FBX))
            {
                errorStringBuilder.Append("Please provide the FBX path of the character!\n\n");
                isErrorDetected = true;
            }

            if (string.IsNullOrEmpty(characterInfo.SaveNewCharacterTo))
            {
                errorStringBuilder.Append("Please provide a folder location for the new character!\n\n");
                isErrorDetected = true;
            }

            if (!characterInfo.CreateNewAnimatorOverrideController && string.IsNullOrEmpty(characterInfo.AnimatorOverrideController))
            {
                errorStringBuilder.Append("Please provide a RuntimeAnimatorController!\n\n");
                isErrorDetected = true;
            }

            if (characterInfo.CreateNewAnimatorOverrideController && string.IsNullOrEmpty(characterInfo.SaveAnimatorOverrideControllerTo))
            {
                errorStringBuilder.Append("You want to create a new animator override controller, but no folder path for it is specified!\n\n");
                isErrorDetected = true;
            }

            if (isErrorDetected)
            {
                throw new Exception(errorStringBuilder.ToString());
            }
        }

        private void CheckIfEditorReady()
        {
            isEditorReady &= !EditorApplication.isCompiling;
            isEditorReady &= !EditorApplication.isPlayingOrWillChangePlaymode;
            isEditorReady &= !EditorApplication.isUpdating;
        }

        private string CreateInstructionsString()
        {
            StringBuilder instructionsStringBuilder = new StringBuilder();
            instructionsStringBuilder.Append("After you create new character, don't forget to add this character info to \n" +
                "1. Localization\n" +
                "2. Audio system (footsteps)");
            return instructionsStringBuilder.ToString();
        }
    }
}
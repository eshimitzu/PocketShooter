using System;
using System.Collections.Generic;
using System.Text;
using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Character;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.CharacterSetupUtility
{
    public class CharacterPrefabManager
    {
        private const string MaleCharacterTemplatePrefabPath = "Assets/Prefabs/CharacterSetupUtility/MaleCharacterTemplate.prefab";
        private const string CommonAnimatorControllerPath = "Assets/Art/Characters/Animation/AnimatorsOverride/Rambo(main).controller";

        public void CreateCharacter(CharacterInfo characterInfo)
        {
            GameObject templateCharacterGameObject = CreateTemplateCharacterFromUnpackedPrefab();
            GameObject fbxGameObject = InstantiateUnpackedFBXPrefab(characterInfo.FBX);

            try
            {
                SetupCharacter(characterInfo, templateCharacterGameObject, fbxGameObject);
            }
            catch
            {
                AbortCharacterCreation(templateCharacterGameObject);
            }

            if (templateCharacterGameObject != null)
            {
                SaveNewCharacterPrefab(templateCharacterGameObject, characterInfo.SaveNewCharacterTo);

                DestroyGameObjectImmediately(templateCharacterGameObject);
            }
        }

        public void UpdateBones(SkinnedMeshRenderer targetSkin, Transform sourceRootBone)
        {
            Transform[] newBones = new Transform[targetSkin.bones.Length];
            Transform[] sourceBones = sourceRootBone.GetComponentsInChildren<Transform>();

            int matchedBonesCount = 0;

            for (int i = 0; i < targetSkin.bones.Length; i++)
            {
                foreach (var sourceBone in sourceBones)
                {
                    if (sourceBone.name == targetSkin.bones[i].name)
                    {
                        newBones[i] = sourceBone;
                        matchedBonesCount += 1;
                        continue;
                    }
                }
            }

            if (matchedBonesCount < newBones.Length)
            {
                throw new Exception("Can't update new SkinnedMeshRenderer's bone array! \n " +
                    "Some bones can't be found in the sourceRootBone array");
            }

            targetSkin.bones = newBones;
        }

        private GameObject CreateTemplateCharacterFromUnpackedPrefab()
        {
            GameObject maleCharacterTemplatePrefab = (GameObject)AssetDatabase.LoadAssetAtPath(MaleCharacterTemplatePrefabPath, typeof(GameObject));

            if (maleCharacterTemplatePrefab == null)
            {
                throw new Exception("MaleCharacterTemplatePrefab can't be found!");
            }

            GameObject maleCharacterTemplatePrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(maleCharacterTemplatePrefab);
            PrefabUtility.UnpackPrefabInstance(maleCharacterTemplatePrefabInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            return maleCharacterTemplatePrefabInstance;
        }

        private GameObject InstantiateUnpackedFBXPrefab(string fbxObjectPath)
        {
            GameObject fbxPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(fbxObjectPath, typeof(GameObject));

            if (fbxPrefab == null)
            {
                throw new Exception("FBX prefab can't be found!");
            }

            GameObject fbxInstance = (GameObject)PrefabUtility.InstantiatePrefab(fbxPrefab);
            PrefabUtility.UnpackPrefabInstance(fbxInstance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            return fbxInstance;
        }

        private void SetupCharacter(CharacterInfo characterInfo, GameObject templateCharacterObject, GameObject fbxGameObject)
        {
                templateCharacterObject.name = characterInfo.TrooperClass.ToString();

                SetupTrooperSkinnedMesh(characterInfo, templateCharacterObject, fbxGameObject);

                DestroyGameObjectImmediately(fbxGameObject);

                Animator trooperAnimator = templateCharacterObject.GetComponentInChildren<Animator>();
                SetupTrooperAnimator(characterInfo, trooperAnimator);

                FootstepsAudioManager footstepsAudioManager = templateCharacterObject.GetComponentInChildren<FootstepsAudioManager>();
                footstepsAudioManager.SetTrooperClass(TrooperClass.Rambo);
        }

        private void SetupTrooperSkinnedMesh(CharacterInfo characterInfo, GameObject templateCharacterGameObject, GameObject fbx)
        {
            GameObject trooperChildGameObject = templateCharacterGameObject.transform.Find("Trooper").gameObject;

            GameObject skinnedMeshGameObject = fbx.GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
            skinnedMeshGameObject.transform.parent = trooperChildGameObject.transform;
            skinnedMeshGameObject.name = characterInfo.TrooperClass.ToString();

            SkinnedMeshRenderer meshRenderer = skinnedMeshGameObject.GetComponent<SkinnedMeshRenderer>();
            meshRenderer.quality = SkinQuality.Bone4;
            Transform rootBone = trooperChildGameObject.transform.Find("Trooper_Root").transform;
            Transform hipsBone = rootBone.Find("Trooper_Hips").transform;

            TrooperAvatar trooperAvatar = templateCharacterGameObject.GetComponent<TrooperAvatar>();
            Renderer[] meshes = new Renderer[1];
            meshes[0] = meshRenderer;
            trooperAvatar.SetMeshesArray(meshes);

            Material[] materials = new Material[1];
            Material selectedmaterial = (Material)AssetDatabase.LoadAssetAtPath(characterInfo.Material, typeof(Material));
            materials[0] = selectedmaterial;
            meshRenderer.materials = materials;

            UpdateBones(meshRenderer, rootBone);

            meshRenderer.rootBone = hipsBone;
        }

        private void SetupTrooperAnimator(CharacterInfo characterInfo, Animator trooperAnimator)
        {
            AnimatorOverrideController animatorOverrideController;
            RuntimeAnimatorController runTimeAnimatorController = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath(
                CommonAnimatorControllerPath, typeof(RuntimeAnimatorController));

            if (runTimeAnimatorController == null)
            {
                throw new Exception("Default AnimatorController can't be found!");
            }

            if (characterInfo.CreateNewAnimatorOverrideController)
            {
                animatorOverrideController = CreateTrooperAnimatorOverride(characterInfo, runTimeAnimatorController);
            }
            else
            {
                animatorOverrideController = GetExistingTrooperAnimatorOverride(characterInfo.AnimatorOverrideController);
            }

            if (animatorOverrideController != null)
            {
                trooperAnimator.runtimeAnimatorController = animatorOverrideController;
            }
        }

        private AnimatorOverrideController CreateTrooperAnimatorOverride(CharacterInfo characterInfo, RuntimeAnimatorController defaultAnimatorController)
        {
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(defaultAnimatorController);

            List<KeyValuePair<AnimationClip, AnimationClip>> overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(animatorOverrideController.overridesCount);

            animatorOverrideController.GetOverrides(overrides);
            int matchingOverrides = 0;

            string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { characterInfo.AnimationsFolder });

            foreach (string guid in guids)
            {
                AnimationClip animationClip = (AnimationClip)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(AnimationClip));

                for (int i = 0; i < overrides.Count; ++i)
                {
                    if (string.Equals(animationClip.name, overrides[i].Key.name))
                    {
                        overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, animationClip);
                        animatorOverrideController.ApplyOverrides(overrides);
                        matchingOverrides += 1;
                    }
                }
            }

            if (matchingOverrides == 0)
            {
                bool okPressed = CharacterSetupWindow.ShowWarningWithOkAndCancel(
                    "No matching overrides found in the animations folder! \n" +
                    "Do you still want to create a character with an empty Animator Override Controller?");

                if (!okPressed)
                {
                    throw new Exception("Try providing another folder");
                }
            }

            string overrideControllerName = GetNewOverrideControllerName(characterInfo.AnimationsFolder);

            string overrideControllerPath = GetNewOverrideControllerPath(characterInfo.SaveAnimatorOverrideControllerTo, overrideControllerName);

            AssetDatabase.CreateAsset(animatorOverrideController, overrideControllerPath);

            return animatorOverrideController;
        }

        private string GetNewOverrideControllerName(string animationsFolder)
        {
            string[] animationsFolderPathWords = animationsFolder.Split('/');
            Array.Reverse(animationsFolderPathWords);
            return animationsFolderPathWords[0];
        }

        private string GetNewOverrideControllerPath(string animatorControllerFolder, string animatorOverrideName)
        {
            StringBuilder overrideControllerPath = new StringBuilder();
            overrideControllerPath.Append(animatorControllerFolder);
            overrideControllerPath.Append("/");
            overrideControllerPath.Append(animatorOverrideName);
            overrideControllerPath.Append(".overrideController");
            return overrideControllerPath.ToString();
        }

        private AnimatorOverrideController GetExistingTrooperAnimatorOverride(string animationOverrideControllerPath)
        {
            return (AnimatorOverrideController)AssetDatabase.LoadAssetAtPath(animationOverrideControllerPath, typeof(AnimatorOverrideController));
        }

        private void SaveNewCharacterPrefab(GameObject templateCharacterGameObject, string saveToPath)
        {
            string newPrefabPath = GetNewCharacterPrefabPath(templateCharacterGameObject.name, saveToPath);
            PrefabUtility.SaveAsPrefabAsset(templateCharacterGameObject, newPrefabPath);
        }

        private string GetNewCharacterPrefabPath(string prefabName, string saveToPath)
        {
            StringBuilder prefabsPathStringBuilder = new StringBuilder(100);
            prefabsPathStringBuilder.Append(saveToPath);
            prefabsPathStringBuilder.Append("/");
            prefabsPathStringBuilder.Append(prefabName);
            prefabsPathStringBuilder.Append(".prefab");

            string newCharacterPrefabPath = prefabsPathStringBuilder.ToString();

            return AssetDatabase.GenerateUniqueAssetPath(prefabsPathStringBuilder.ToString());
        }

        private void DestroyGameObjectImmediately(GameObject objectToDestroy)
        {
            if (objectToDestroy != null)
            {
                GameObject.DestroyImmediate(objectToDestroy);
            }
        }

        private void AbortCharacterCreation(GameObject templateGameObject)
        {
            DestroyGameObjectImmediately(templateGameObject);
        }
    }
}
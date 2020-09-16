using System;
using System.Text;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Networking.Actors;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.CharacterSetupUtility
{
    public class TroopersConfigManager
    {
        private const string BotCharacterPrefabPath = "Assets/Prefabs/Character/BotCharacter.prefab";
        private const string TroopersConfigPath = "Assets/Settings/TroopersConfig.asset";

        public void AddCharacterToTroopersConfig(CharacterInfo characterInfo)
        {
            string newTrooperPrefabPath = GetNewTrooperPrefabPath(characterInfo);

            TrooperAvatar avatar;
            try
            {
                avatar = GetNewTrooperPrefab(newTrooperPrefabPath).GetComponent<TrooperAvatar>();
            }
            catch
            {
                throw new Exception("Can't add new Trooper to TroopersConfig. \n" +
                    "Character Prefab is null or it doesn't contain TrooperAvatar component");
            }

            TrooperConfig trooperConfig = null;
            BotCharacter botCharacter = GetBotCharacter();
            TroopersConfig troopersConfig = GetTroopersConfig();

            if (botCharacter != null && avatar != null && troopersConfig != null)
            {
                trooperConfig = new TrooperConfig(characterInfo.TrooperClass, avatar, botCharacter);
                troopersConfig.AddTrooperToTroopersConfig(trooperConfig);
            }
        }

        private string GetNewTrooperPrefabPath(CharacterInfo characterInfo)
        {
            StringBuilder prefabsPathStringBuilder = new StringBuilder(100);
            prefabsPathStringBuilder.Append(characterInfo.SaveNewCharacterTo);
            prefabsPathStringBuilder.Append("/");
            prefabsPathStringBuilder.Append(characterInfo.TrooperClass);
            prefabsPathStringBuilder.Append(".prefab");
            return prefabsPathStringBuilder.ToString();
        }

        private GameObject GetNewTrooperPrefab(string newPrefabPath)
        {
            return (GameObject)AssetDatabase.LoadAssetAtPath(newPrefabPath, typeof(GameObject));
        }

        private BotCharacter GetBotCharacter()
        {
            GameObject botCharacterPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(BotCharacterPrefabPath, typeof(GameObject));

            if (botCharacterPrefab == null)
            {
                throw new Exception("BotCharacter GameObject doesn't exist at the specified location! TroopersConfig modification cancelled!");
            }

            return botCharacterPrefab.GetComponent<BotCharacter>();
        }

        private TroopersConfig GetTroopersConfig()
        {
            TroopersConfig config = (TroopersConfig)AssetDatabase.LoadAssetAtPath(TroopersConfigPath, typeof(TroopersConfig));

            if (config == null)
            {
                throw new Exception("TroopersConfig object not found at the specified location! TroopersConfig modification cancelled!");
            }

            return config;
        }
    }
}
using System;
using Heyworks.PocketShooter.UI;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter.CharacterSetupUtility
{
    public class IconsFactoryManager
    {
        private const string IconsFactoryConfigPath = "Assets/Settings/UI/IconsFactory.asset";

        public void AddCharacterToIconsFactory(CharacterInfo characterInfo)
        {
            Sprite trooperSprite;
            try
            {
                trooperSprite = GetNewTrooperSprite(characterInfo.RosterIcon);
            }
            catch
            {
                Debug.Log(characterInfo.RosterIcon);
                throw new Exception("Can't add new Trooper Sprite to IconsFactory. \n" +
                    "No sprite can be found at the specified location");
            }

            IconsFactory iconsFactoryConfig = GetIconsFactoryConfig();

            iconsFactoryConfig.AddTrooperToTrooperIcons(characterInfo.TrooperClass, trooperSprite);
        }

        private Sprite GetNewTrooperSprite(string newSpritePath)
        {
            return (Sprite)AssetDatabase.LoadAssetAtPath(newSpritePath, typeof(Sprite));
        }

        private IconsFactory GetIconsFactoryConfig()
        {
            IconsFactory iconsFactory = (IconsFactory)AssetDatabase.LoadAssetAtPath(IconsFactoryConfigPath, typeof(IconsFactory));

            if (iconsFactory == null)
            {
                throw new Exception("IconsFactory object not found at the specified location! IconsFactory modification cancelled!");
            }

            return iconsFactory;
        }
    }
}
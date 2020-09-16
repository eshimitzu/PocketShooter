using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Entities;
using UnityEngine;

namespace Heyworks.PocketShooter.UI
{
    [CreateAssetMenu(fileName = "IconsFactory", menuName = "Heyworks/UI/Icons Factory")]
    public class IconsFactory : ScriptableObject
    {
        [SerializeField]
        private List<PlaceholderIcon> placeholderIcons;

        [SerializeField]
        private List<TrooperIcon> trooperIcons;

        [SerializeField]
        private List<WeaponIcon> weaponIcons;

        [SerializeField]
        private List<WeaponIcon> weaponSmallIcons;

        [SerializeField]
        private List<HelmetIcon> helmetIcons;

        [SerializeField]
        private List<HelmetIcon> helmetSmallIcons;

        [SerializeField]
        private List<ArmorIcon> armorIcons;

        [SerializeField]
        private List<ArmorIcon> armorSmallIcons;

        [SerializeField]
        private List<WeaponIcon> weaponWhiteIcons;

        [SerializeField]
        private List<HelmetIcon> helmetWhiteIcons;

        [SerializeField]
        private List<ArmorIcon> armorWhiteIcons;

        [SerializeField]
        private List<OffensiveIcon> offensiveIcons;

        [SerializeField]
        private List<SupportIcon> supportIcons;

        [SerializeField]
        private Sprite goldIcon;

        [SerializeField]
        private Sprite cashIcon;

        public Sprite SpriteForItem(IRosterItem item)
        {
            if (item is ITrooperItem trooperItem)
            {
                TrooperIcon icon = trooperIcons.Find(f => f.TrooperClass == trooperItem.Class);
                return icon?.Sprite;
            }

            if (item is IWeaponItem weaponItem)
            {
                WeaponIcon icon = weaponIcons.Find(f => f.WeaponName == weaponItem.Name);
                return icon?.Sprite;
            }

            if (item is IArmorItem armorItem)
            {
                ArmorIcon icon = armorIcons.Find(e => e.ArmorName == armorItem.Name);
                return icon?.Sprite;
            }

            if (item is IHelmetItem helmetItem)
            {
                HelmetIcon icon = helmetIcons.Find(e => e.HelmetName == helmetItem.Name);
                return icon?.Sprite;
            }

            return null;
        }

        public Sprite SpriteForItem(IContentIdentity item)
        {
            if (item is TrooperIdentity trooperItem)
            {
                TrooperIcon icon = trooperIcons.Find(f => f.TrooperClass == trooperItem.Class);
                return icon?.Sprite;
            }

            if (item is WeaponIdentity weaponItem)
            {
                WeaponIcon icon = weaponIcons.Find(f => f.WeaponName == weaponItem.Name);
                return icon?.Sprite;
            }

            if (item is ArmorIdentity armorItem)
            {
                ArmorIcon icon = armorIcons.Find(e => e.ArmorName == armorItem.Name);
                return icon?.Sprite;
            }

            if (item is HelmetIdentity helmetItem)
            {
                HelmetIcon icon = helmetIcons.Find(e => e.HelmetName == helmetItem.Name);
                return icon?.Sprite;
            }

            if (item is ResourceIdentity resourceItem)
            {
                return resourceItem.Gold > 0 ? goldIcon : cashIcon;
            }

            if (item is OffensiveIdentity offensiveItem)
            {
                OffensiveIcon icon = offensiveIcons.Find(e => e.OffensiveName == offensiveItem.Name);
                return icon?.Sprite;
            }

            if (item is SupportIdentity supportItem)
            {
                SupportIcon icon = supportIcons.Find(e => e.SupportName == supportItem.Name);
                return icon?.Sprite;
            }

            return null;
        }

        public Sprite SmallSpriteForWeaponItem(WeaponName weaponName)
        {
            WeaponIcon icon = weaponSmallIcons.Find(f => f.WeaponName == weaponName);
            return icon?.Sprite;
        }

        public Sprite SmallSpriteForHelmetItem(HelmetName helmetName)
        {
            HelmetIcon icon = helmetSmallIcons.Find(f => f.HelmetName == helmetName);
            return icon?.Sprite;
        }

        public Sprite SmallSpriteForArmorItem(ArmorName armorName)
        {
            ArmorIcon icon = armorSmallIcons.Find(f => f.ArmorName == armorName);
            return icon?.Sprite;
        }

        public Sprite WhiteSpriteForHelmetItem(HelmetName helmetName)
        {
            HelmetIcon icon = helmetWhiteIcons.Find(e => e.HelmetName == helmetName);
            return icon?.Sprite;
        }

        public Sprite WhiteSpriteForArmotItem(ArmorName armorName)
        {
            ArmorIcon icon = armorWhiteIcons.Find(e => e.ArmorName == armorName);
            return icon?.Sprite;
        }

        public Sprite WhiteSpriteForWeaponItem(WeaponName weaponName)
        {
            WeaponIcon icon = weaponWhiteIcons.Find(f => f.WeaponName == weaponName);
            return icon?.Sprite;
        }

        public Sprite SpriteForPlaceholderCard(RosterType rosterType)
        {
            Sprite result = placeholderIcons.Find(e => e.RosterType == rosterType)?.Sprite;

            if (result == null)
            {
                Throw.Argument(nameof(rosterType));
            }

            return result;
        }

        public void AddTrooperToTrooperIcons(TrooperClass trooperClass, Sprite trooperSprite)
        {
            TrooperIcon newTrooperIcon = new TrooperIcon() { TrooperClass = trooperClass, Sprite = trooperSprite };

            trooperIcons.Add(newTrooperIcon);
        }

        [Serializable]
        private class TrooperIcon
        {
            public TrooperClass TrooperClass;
            public Sprite Sprite;
        }

        [Serializable]
        private class WeaponIcon
        {
            public WeaponName WeaponName;
            public Sprite Sprite;
        }

        [Serializable]
        private class HelmetIcon
        {
            public HelmetName HelmetName;
            public Sprite Sprite;
        }

        [Serializable]
        private class ArmorIcon
        {
            public ArmorName ArmorName;
            public Sprite Sprite;
        }

        [Serializable]
        private class OffensiveIcon
        {
            public OffensiveName OffensiveName;
            public Sprite Sprite;
        }

        [Serializable]
        private class SupportIcon
        {
            public SupportName SupportName;
            public Sprite Sprite;
        }

        [Serializable]
        private class PlaceholderIcon
        {
            public RosterType RosterType;
            public Sprite Sprite;
        }
    }
}
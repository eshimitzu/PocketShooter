using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.UI.Localization;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyMainCard : MonoBehaviour
    {
        private const int StatsCount = 4;
        private const string FormatF0 = "F0";
        private const string FormatF1 = "F1";

        [SerializeField]
        private LobbyStatsCell powerCell;

        [SerializeField]
        private LobbyStatsCell maxPowerCell;

        [SerializeField]
        private LobbyStatsCell levelCell;

        [SerializeField]
        private LobbyStatsCell maxLevelCell;

        [SerializeField]
        private List<Image> starsCurrent;

        [SerializeField]
        private Image starsArrow;

        [SerializeField]
        private List<Image> starsNext;

        [SerializeField]
        private LobbyColorsPreset colorsPreset;

        [SerializeField]
        private LobbyStatsCell[] stats;

        [SerializeField]
        private Icons icons;

        [SerializeField]
        private Colors upgradeColors;

        public void Setup(IRosterItem item)
        {
            Assert.AreEqual(stats.Length, StatsCount);

            // TODO: v.shimkovich use string numbers cache / format
            switch (item)
            {
                case IArmyItem armyItem:
                    bool isMaxed = armyItem.IsMaxGrade && armyItem.IsMaxLevel;
                    (ItemStats nextStats, int? nextLevel, int? nextMaxLevel, Grade? nextGrade) = isMaxed
                        ? (null, null, null, null)
                        : armyItem.IsMaxLevel
                            ? (armyItem.NextGradeStats, (int?)(null), (int?)armyItem.NexGradeMaxLevel, (Grade?)(armyItem.Grade + 1))
                            : (armyItem.NextLevelStats, armyItem.Level + 1, null, null);
                    SetupImpl(armyItem, nextLevel, nextMaxLevel, nextGrade, nextStats, upgradeColors.NextUpgrade);
                    break;
                case IProductItem productItem:
                    SetupImpl(productItem, productItem.MaxGradeMaxLevel, productItem.MaxGradeMaxLevel, productItem.MaxGrade, productItem.MaxStats, upgradeColors.MaxUpgrade);
                    break;
                case ILobbyRosterItem rItem:
                    SetupImpl(rItem, null, null, null, null, upgradeColors.NextUpgrade);
                    break;
            }
        }

        private void SetupImpl(IRosterItem item, int? nextLevel, int? nextMaxLevel, Grade? nextGrade, ItemStats nextStats, Color upgradeColor)
        {
            powerCell.Setup(null, LocKeys.Power, item.Power.ToString(), upgradeColor, nextStats?.Power.ToString());
            maxPowerCell.Setup(null, LocKeys.MaxPower, item.MaxPower.ToString(), upgradeColor);
            levelCell.Setup(null, LocKeys.Level, item.Level.ToString(), upgradeColor, nextLevel?.ToString());
            maxLevelCell.Setup(null, LocKeys.MaxLevel, item.MaxLevel.ToString(), upgradeColor, nextMaxLevel?.ToString());

            for (var i = 0; i < starsCurrent.Count; i++)
            {
                Image star = starsCurrent[i];
                star.color = colorsPreset.GetStarsColor((int)item.Grade >= i);
            }

            starsArrow.enabled = nextGrade.HasValue;

            for (var i = 0; i < starsNext.Count; i++)
            {
                var star = starsNext[i];
                star.color = nextGrade.HasValue ? colorsPreset.GetStarsColor((int)nextGrade.Value >= i) : star.color;
                star.gameObject.SetActive(nextGrade.HasValue);
            }

            int currentIndex = 0;
            switch (item)
            {
                case ITrooperItem trooper:
                    SetStatsValue(ref currentIndex, icons.Health, LocKeys.Health, trooper.Health, upgradeColor, nextStats?.Health);
                    SetStatsValue(ref currentIndex, icons.Armor, LocKeys.Armor, trooper.Armor, upgradeColor, nextStats?.Armor);
                    SetStatsValue(ref currentIndex, icons.Speed, LocKeys.Movement, trooper.Movement, upgradeColor, (float?)nextStats?.Movement, FormatF1);
                    break;
                case IWeaponItem weapon:
                    SetStatsValue(ref currentIndex, icons.Attack, LocKeys.Attack, weapon.Attack, upgradeColor, nextStats?.Attack);
                    SetStatsValue(ref currentIndex, icons.Distance, LocKeys.Distance, weapon.Distance, upgradeColor, nextStats?.Distance);
                    SetStatsValue(ref currentIndex, icons.Capacity, LocKeys.Capacity, weapon.Capacity, upgradeColor, nextStats?.Capacity);
                    SetStatsValue(ref currentIndex, icons.Reload, LocKeys.Reload, weapon.Reload, upgradeColor, (float?)nextStats?.Reload, FormatF1);
                    break;
                case IHelmetItem helmet:
                    SetStatsValue(ref currentIndex, icons.Health, LocKeys.Health, helmet.Health, upgradeColor, nextStats?.Health);
                    break;
                case IArmorItem armor:
                    SetStatsValue(ref currentIndex, icons.Armor, LocKeys.Armor, armor.Armor, upgradeColor, nextStats?.Armor);
                    break;
            }

            for (int i = 0; i < StatsCount; i++)
            {
                stats[i].gameObject.SetActive(i < currentIndex);
            }
        }

        private void SetStatsValue(ref int index, Sprite icon, string title, float currentValue, Color nextValueColor, float? nextValue = null, string format = FormatF0)
        {
            if (currentValue == 0)
            {
                return;
            }

            nextValue = nextValue == currentValue ? null : nextValue;
            stats[index].Setup(icon, title, currentValue.ToString(format), nextValueColor, nextValue?.ToString(format));

            index++;
        }

        [Serializable]
        private class Icons
        {
            [SerializeField]
            internal Sprite Health;

            [SerializeField]
            internal Sprite Armor;

            [SerializeField]
            internal Sprite Speed;

            [SerializeField]
            internal Sprite Capacity;

            [SerializeField]
            internal Sprite Distance;

            [SerializeField]
            internal Sprite Attack;

            [SerializeField]
            internal Sprite Reload;
        }

        [Serializable]
        public class Colors
        {
            [SerializeField]
            internal Color NextUpgrade = Color.green;

            [SerializeField]
            internal Color MaxUpgrade = Color.yellow;
        }
}
}
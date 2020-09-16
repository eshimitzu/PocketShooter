using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Entities;
using UnityEngine;

namespace Heyworks.PocketShooter.UI
{
    public class TrooperSelectionHandler
    {
        private const string RosterLastTrooperKey = "RosterLastTrooper";

        private readonly MetaGame metaGame;
        private readonly LobbyRosterPresenter trooperRoster;

        public TrooperClass CurrentTrooperClass
        {
            get => (TrooperClass)PlayerPrefs.GetInt(RosterLastTrooperKey, 0);
            set => PlayerPrefs.SetInt(RosterLastTrooperKey, (int)value);
        }

        public Trooper CurrentTrooper => metaGame.Army.Troopers.FirstOrDefault(_ => _.Class == CurrentTrooperClass);

        public TrooperSelectionHandler(MetaGame game)
        {
            metaGame = game;
        }

        public int GetCurrentIndex(ref List<ILobbyRosterItem> rosterItems, RosterType rosterType)
        {
            int currentIndex = 0;
            Trooper currentTrooper = CurrentTrooper;
            for (int i = 0; i < rosterItems.Count; i++)
            {
                if (currentTrooper != null)
                {
                    if (rosterItems[i] is IHelmetItem helmetItem)
                    {
                        if (helmetItem.Name == currentTrooper.CurrentHelmet.Name)
                        {
                            currentIndex = i;
                            break;
                        }
                    }
                    else if (rosterItems[i] is IArmorItem armorItem)
                    {
                        if (armorItem.Name == currentTrooper.CurrentArmor.Name)
                        {
                            currentIndex = i;
                            break;
                        }
                    }
                    else if (rosterItems[i] is IWeaponItem weaponItem)
                    {
                        if (weaponItem.Name == currentTrooper.CurrentWeapon.Name)
                        {
                            currentIndex = i;
                            break;
                        }
                    }
                }

                if (rosterItems[i] is ITrooperItem trooperItem)
                {
                    if (trooperItem.Class == CurrentTrooperClass)
                    {
                        currentIndex = i;
                        break;
                    }
                }
            }

            return currentIndex;
        }

        public bool HasTrooperInArmy(TrooperClass trooperClass)
        {
            Trooper currentTrooper = metaGame.Army.Troopers.FirstOrDefault(_ => _.Class == trooperClass);
            return currentTrooper != null;
        }
    }
}
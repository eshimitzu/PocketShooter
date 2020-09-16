using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Entities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyRosterPresenter : ILobbyRosterPresenter
    {
        private const int LeftFakeCardsCount = 6;
        private const int RightFakeCardsCount = 6;

        private readonly LobbyRosterView rosterView;
        private readonly RosterScroll rosterScroll;
        private readonly IconsFactory itemsFactory;
        private readonly TrooperSelectionHandler trooperSelection;
        private readonly MetaGame metaGame;

        private int lastSelectedIndex;
        private ILobbyRosterItem lastItem;

        public RosterType RosterType { get; private set; }

        private bool firstFrame;
        private GameObject trooper;
        private List<ILobbyCardPresenter> cards = new List<ILobbyCardPresenter>();
        private bool isAutoSavingCurrentCell;
        private List<ILobbyRosterItem> rosterItems;

        public ILobbyCardPresenter SelectedCard { get; protected set; }

        public LobbyRosterView RosterView => rosterView;

        public List<ILobbyCardPresenter> Cards => cards;

        public TrooperClass CurrentTrooperClass => trooperSelection.CurrentTrooperClass;

        public event Action<ILobbyCardPresenter> OnSelectedCardChanged;

        public LobbyRosterPresenter(
            LobbyRosterView rosterView,
            IconsFactory itemsFactory,
            RosterType rosterType,
            List<ILobbyRosterItem> rosterItems,
            TrooperSelectionHandler trooperSelection,
            MetaGame metaGame)
        {
            this.rosterView = rosterView;
            this.itemsFactory = itemsFactory;
            RosterType = rosterType;
            this.trooperSelection = trooperSelection;
            this.metaGame = metaGame;

            rosterScroll = rosterView.RosterScroll;

            rosterView.RosterScroll.OnScrollIndexUpdated += RosterScroll_OnScrollIndexUpdated;

            SetupCards(rosterItems, false);
        }

        public void Dispose()
        {
            rosterView.RosterScroll.OnScrollIndexUpdated -= RosterScroll_OnScrollIndexUpdated;
        }

        public void SetupCards(List<ILobbyRosterItem> rosterItems, bool isSetCurrentToLast)
        {
            this.rosterItems = rosterItems;

            var cardObjects = new List<LobbyRosterEmptyCard>();
            rosterView.GetComponentsInChildren(cardObjects);
            for (var i = 0; i < cardObjects.Count; i++)
            {
                var card = cardObjects[i];
                Object.Destroy(card.gameObject);
            }

            foreach (var card in cards)
            {
                card.Dispose();
            }

            cards.Clear();
            SelectedCard = null;

            RectOffset padding = rosterScroll.ContentLayout.padding;
            padding.left =
                4 - (int)(rosterScroll.RosterCardWidth + rosterScroll.RosterCardDelta) * LeftFakeCardsCount;

            padding.right =
                4 - (int)(rosterScroll.RosterCardWidth + rosterScroll.RosterCardDelta) * RightFakeCardsCount;

            for (int i = 0; i < LeftFakeCardsCount; i++)
            {
                var card = Object.Instantiate(rosterView.FakeCardPrefab, rosterView.ContentRoot);
                card.Placeholder.sprite = itemsFactory.SpriteForPlaceholderCard(RosterType);
            }

            for (int i = 0; i < rosterItems.Count; i++)
            {
                LobbyRosterCard card = Object.Instantiate(rosterView.CardPrefab, rosterView.ContentRoot);
                card.name = card.name + " " + i;
                var presenter = new LobbyCardPresenter(card, i, rosterItems[i], itemsFactory);
                cards.Add(presenter);

                card.OnCardClicked += () => { UpdateSelected(presenter.Index); };
            }

            for (int i = 0; i < RightFakeCardsCount; i++)
            {
                var card = Object.Instantiate(rosterView.FakeCardPrefab, rosterView.ContentRoot);
                card.Placeholder.sprite = itemsFactory.SpriteForPlaceholderCard(RosterType);
            }

            if (RosterView.gameObject.activeSelf)
            {
                foreach (var card in cards)
                {
                    UpdateCardWeaponIcon(card);
                }
            }

            int currentIndex = trooperSelection.GetCurrentIndex(ref rosterItems, RosterType);

            UpdateSelected(currentIndex, false, isSetCurrentToLast);
        }

        public void UpdateSelected(string itemName)
        {
            foreach (var card in cards)
            {
                if (card.Item.ItemName == itemName)
                {
                    UpdateSelected(card.Index);
                    break;
                }
            }
        }

        public void SetVisibleView(bool isVisible)
        {
            RosterView.gameObject.SetActive(isVisible);

            if (isVisible)
            {
                foreach (var card in cards)
                {
                    UpdateCardWeaponIcon(card);
                }
            }
        }

        public void Refresh()
        {
            rosterItems.Sort(
                (a, b) =>
                {
                    int result;
                    if (a.Product != null && b.Product != null)
                    {
                        result = (a.Product.IsLocked ? a.Product.PlayerLevelForUnlock : 0)
                            .CompareTo(b.Product.IsLocked ? b.Product.PlayerLevelForUnlock : 0);
                    }
                    else
                    {
                        result = (a.Product == null ? 0 : 1).CompareTo(b.Product == null ? 0 : 1);
                    }

                    if (result == 0)
                    {
                        result = a.Power.CompareTo(b.Power);
                    }

                    return result;
                });

            SetupCards(rosterItems, true);
        }

        private void UpdateSelected(int index = 0, bool animated = true, bool isSetCurrentToLast = false)
        {
            index = Mathf.Clamp(index, 0, cards.Count - 1);

            int selectedIndex = index;
            int scroolIndex = ScrollIndex(index);
            bool canSetCurrentToEquipped = isSetCurrentToLast && lastItem != null;

            if (canSetCurrentToEquipped)
            {
                for (int i = 0; i < rosterItems.Count; i++)
                {
                    if (rosterItems[i].ItemName == lastItem.ItemName)
                    {
                        selectedIndex = i;
                        break;
                    }
                }

                scroolIndex = lastSelectedIndex;
            }

            SelectedCard?.SetSelected(false);
            SelectedCard = cards[selectedIndex];
            SelectedCard.SetSelected(true);

            lastItem = SelectedCard?.Item;
            lastSelectedIndex = rosterScroll.CurrentIndex;

            if (SelectedCard.Item is ITrooperItem trooperItem)
            {
                trooperSelection.CurrentTrooperClass = trooperItem.Class;
            }

            if (!canSetCurrentToEquipped)
            {
                if (!animated)
                {
                    rosterScroll.ScrollToIndex(scroolIndex, false);
                }

                rosterScroll.ScrollToIndex(scroolIndex);
            }

            if (rosterView.gameObject.activeInHierarchy)
            {
                OnSelectedCardChanged?.Invoke(SelectedCard);
            }

            UpdateCardWeaponIcon(SelectedCard);
        }

        private void RosterScroll_OnScrollIndexUpdated(int index)
        {
            if (!firstFrame)
            {
                firstFrame = true;
                int currentIndex = trooperSelection.GetCurrentIndex(ref rosterItems, RosterType);
                UpdateSelected(currentIndex, true);
            }
        }

        private void UpdateCardWeaponIcon(ILobbyCardPresenter card)
        {
            if (card.Item is ITrooperItem trooperItem)
            {
                Trooper currentTrooper = metaGame.Army.Troopers.FirstOrDefault(_ => _.Class == trooperItem.Class);

                bool hasTrooperInArmy = currentTrooper != null;

                if (hasTrooperInArmy)
                {
                    card.UpdateWeaponIcon(currentTrooper.CurrentWeapon.Name);
                }

                card.SetVisibleWeaponIcon(hasTrooperInArmy);
            }
            else
            {
                card.SetVisibleWeaponIcon(false);
            }
        }

        private int ScrollIndex(int index) => index + LeftFakeCardsCount;
    }
}
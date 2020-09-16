using System;
using UnityEngine;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyConsumablesView : MonoBehaviour
    {
        [SerializeField]
        private LobbyOfferItem grenadeItem;

        [SerializeField]
        private LobbyOfferItem medkitItem;

        [SerializeField]
        private LobbyOfferItem chestItem;

        public LobbyOfferItem GrenadeItem => grenadeItem;

        public LobbyOfferItem MedkitItem => medkitItem;

        public LobbyOfferItem ChestItem => chestItem;
    }
}

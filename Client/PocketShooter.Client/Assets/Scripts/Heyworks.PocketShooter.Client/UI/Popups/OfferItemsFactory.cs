using System.Collections.Generic;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.Popups
{
    [CreateAssetMenu(fileName = "OfferItemsFactory", menuName = "Heyworks/UI/Offers Factory")]
    public class OfferItemsFactory : ScriptableObject
    {
        [SerializeField]
        private List<Banner> banners;

        public Sprite BannerSpriteForOffer(OfferType offerType)
        {
            Banner banner = banners.Find(f => f.OfferType == offerType);
            return banner?.Sprite;
        }

        [System.Serializable]
        private class Banner
        {
            public OfferType OfferType;
            public Sprite Sprite;
        }
    }
}
namespace Heyworks.PocketShooter.Meta.Entities
{
    public class OfferPopupData 
    {
        public OfferPopupData(string offerProductId, float appearanceChance, int discount)
        {
            OfferProductId = offerProductId;
            AppearanceChance = appearanceChance;
            Discount = discount;
        }

        public string OfferProductId { get; }

        public float AppearanceChance { get; }

        public int Discount { get; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class OfferPopupConfig
    {
        [Required]
        public string OfferProductId { get; set; }

        [Required]
        public float AppearanceChance { get; set; }

        [Required]
        public int Discount { get; set; }
    }
}

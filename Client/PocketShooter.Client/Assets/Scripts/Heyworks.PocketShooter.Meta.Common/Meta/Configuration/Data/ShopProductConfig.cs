using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Entities;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class ShopProductConfig
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public Price Price { get; set; }

        [Required]
        public string ContentPackId { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(100)]
        public List<ShopCategory> Category { get; set; } = new List<ShopCategory>()
        {
            ShopCategory.Hard
        };

        [Range(1, int.MaxValue)]
        public int MinPlayerLevel { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxPlayerLevel { get; set; }
    }
}

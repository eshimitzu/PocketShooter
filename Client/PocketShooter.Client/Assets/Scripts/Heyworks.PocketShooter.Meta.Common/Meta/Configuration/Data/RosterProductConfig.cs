using Heyworks.PocketShooter.Meta.Entities;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class RosterProductConfig
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public Price Price { get; set; }

        [Required]
        public string ContentPackId { get; set; }

        [Range(1, int.MaxValue)]
        public int PlayerLevelRequired { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class BotsTrainConfig
    {
        [Range(0, ushort.MaxValue)]
        [Description("Level apply to. If non of level matches - than not limits are applied.")]
        public int Level { get; set; }

        [Range(0.0, 1.0)]
        [Description("How weaker attack will be. 1 is default")]
        public float DamageFactorPercent { get; set; } = 1;

        [Range(0.0, 1.0)]
        [Description("How less damage should take than normal. 1 is default.")]
        public float ProtectionFactorPercent { get; set; } = 1;
    
        [Description("Maximal grade allowed")]
        public Grade MaximalGrade { get; set; } = Grade.Legendary; // Enum.GetMax is more robust
    }
}
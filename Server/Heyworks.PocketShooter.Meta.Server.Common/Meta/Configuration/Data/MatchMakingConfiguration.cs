using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class MatchMakingConfiguration
    {
        // what levels range can participate in same game
        [Range(0, short.MaxValue)]
        [Required]
        public int InitialLevelSpread { get; set; } = 1;

        // after this time player gets the game event if room is not full
        [Range(0,ushort.MaxValue)]
        [Required]
        public int ForcedStartMs { get; set; } = 10_000;

        [Required]
        [Range(0, short.MaxValue)]
        public int LearningMeterBeforePvP { get; set; } = 200; 
    }
}

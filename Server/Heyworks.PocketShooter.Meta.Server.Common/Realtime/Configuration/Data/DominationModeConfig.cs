using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    [Description("Configuration of domination game mode")]
    public class DominationModeConfig
    {        
        [Description("Gets the maximum players in game")]
        [DefaultValueAttribute(10)] // NOTE: may use defaults to avoid copy-pased if need to be different for some maps
        [Range(1, byte.MaxValue)]
        public int MaxPlayers { get; set; } = 10;

        [Description("Gets time to respawn in milliseconds")]
        [DefaultValueAttribute(9000)]
        public int RespawnTimeMs { get; set; } = 9000;
        
        [Description("Gets the amount of \"timeplayers\" to capture the point (timeplayers = playersCount * seconds, real formula is a bit different)")]
        [DefaultValueAttribute(4)]
        public int TimeplayersToCapture { get; set; } = 4;

        [Description("The amount of time to wait before next zones state check in ms. This will affect game speed. To increase check frequency only, combine this with capture speed and points gain.")]
        [DefaultValueAttribute(1000)]
        public int CheckIntervalMs { get; set; } = 1000;

        [Description("Gets or sets the points to win.")]
        [DefaultValueAttribute(1000)]
        public int WinScore { get; set; } = 1000;

        [DefaultValueAttribute(300000)]
        public int GameDurationMs { get; set; } = 300000;

        [Required]
        public MVPConfig MVPConfig { get; set; } = new MVPConfig();

        [Required]
        public GameArmorConfig GameArmorConfig { get; set; } = new GameArmorConfig();
    }
}
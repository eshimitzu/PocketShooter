using System.ComponentModel;

namespace Heyworks.PocketShooter.Realtime.Configuration.Data
{
    /// <summary>
    /// Configuration of armor in game.
    /// </summary>
    public class GameArmorConfig
    {    
        [DefaultValueAttribute(1f)]
        public float Impact { get; set; } = 1f;

        [DefaultValueAttribute(1f)]
        public float DamageFactor { get; set; } = 1f;
    }
}
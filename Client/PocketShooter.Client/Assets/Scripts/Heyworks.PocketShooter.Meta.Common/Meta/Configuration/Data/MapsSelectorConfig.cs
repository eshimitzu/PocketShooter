using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    [Description("Allow to put player onto specific maps during match making")]
    public class MapsSelectorConfig
    {
        
        [Required]
        public MapNames Name { get; set; } = MapNames.Mexico;

        [Range(0, ushort.MaxValue)]
        [Description("Level to allow play from inclusive. If empty than from lowers level.")]
        [DefaultValue(1)]
        public int StartLevel { get; set; } = 1;

        [Range(1, ushort.MaxValue)]
        [Description("Level to allow play to inclusive. If empty than to highest level.")]
        [DefaultValue(ushort.MaxValue)]
        public int EndLevel { get; set; } = ushort.MaxValue;
    }
}
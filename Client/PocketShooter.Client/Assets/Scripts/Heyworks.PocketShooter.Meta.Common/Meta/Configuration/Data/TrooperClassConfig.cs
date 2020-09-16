using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Heyworks.PocketShooter.Meta.Configuration.Data
{
    public class TrooperClassConfig
    {
        public TrooperClass Class { get; set; }

        [MinLength(5)]
        [MaxLength(5)]
        public List<SkillName> Skills { get; set; } = new List<SkillName>();
    }
}

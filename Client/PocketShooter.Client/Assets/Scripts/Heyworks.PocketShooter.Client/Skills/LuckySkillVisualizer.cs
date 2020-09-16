using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Skills
{
    public class LuckySkillVisualizer : SkillVisualizer
    {
        public LuckySkillVisualizer(Skill skillModel, LuckySkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
        }

        // All visualization logic is handle in the effect controller (LuckyEffectController).
    }
}

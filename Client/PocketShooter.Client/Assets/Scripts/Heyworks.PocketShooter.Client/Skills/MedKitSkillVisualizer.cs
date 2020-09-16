using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;

namespace Heyworks.PocketShooter.Skills
{
    public class MedKitSkillVisualizer : SkillVisualizer
    {
        public MedKitSkillVisualizer(Skill skillModel, MedKitSkillSpec spec, NetworkCharacter character)
            : base(skillModel, spec, character)
        {
        }
    }
}

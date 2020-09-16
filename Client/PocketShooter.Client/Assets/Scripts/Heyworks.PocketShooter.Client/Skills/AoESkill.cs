using System.Linq;
using Collections.Pooled;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Skills
{
    public class AoESkill
    {
        private readonly SkillName skillName;
        private readonly LocalCharacter character;
        private readonly AoEInfo area;

        public AoESkill(SkillName skillName, LocalCharacter character, AoEInfo area)
        {
            this.skillName = skillName;
            this.character = character;
            this.area = area;
        }

        public void Execute()
        {
            var victimIds = character.CharacterContainer.GetCharactersInsideCylinder(
                    character.transform.position.x,
                    character.transform.position.y,
                    character.transform.position.z,
                    area.RadiusSqr,
                    area.Height).Where(v => v.Model.Team != character.Model.Team)
                .Select(v => v.Id);

            character.AddCommand(
                new ApplyAoECommandData(
                    character.Id,
                    skillName,
                    new PooledList<EntityId>(victimIds)));
        }
    }
}

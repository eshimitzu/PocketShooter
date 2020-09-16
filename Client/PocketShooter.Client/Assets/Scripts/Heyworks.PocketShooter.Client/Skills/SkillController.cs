using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using UnityEngine.Assertions;

namespace Heyworks.PocketShooter.Skills
{
    public class SkillController
    {
        public SkillName SkillName => Model.Name;

        public OwnedSkill Model { get; }

        public SkillSpec Spec { get; }

        protected LocalCharacter Character { get; }

        public SkillController(OwnedSkill skillModel, SkillSpec skillSpec, LocalCharacter character)
        {
            Assert.IsNotNull(skillModel);
            Assert.IsNotNull(character);
            Assert.IsNotNull(skillSpec);

            Model = skillModel;
            Spec = skillSpec;
            Character = character;
        }

        public virtual void SkillControlOnClick() => Character.UseSkill(SkillName);

        public virtual void SkillControlOnDown()
        {
        }

        public virtual void SkillControlOnUp()
        {
        }

        public virtual void Cancel()
        {
        }
    }
}

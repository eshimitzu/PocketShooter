using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using UnityEngine.Assertions;

namespace Heyworks.PocketShooter.Skills
{
    public class SkillVisualizer
    {
        public SkillName SkillName => Model.Name;

        public Skill Model { get; private set; }

        public SkillSpec Spec { get; private set; }

        protected NetworkCharacter Character { get; private set; }

        public SkillVisualizer(Skill skillModel, SkillSpec spec, NetworkCharacter character)
        {
            Assert.IsNotNull(skillModel);
            Assert.IsNotNull(spec);
            Assert.IsNotNull(character);

            this.Model = skillModel;
            this.Spec = spec;
            this.Character = character;
        }

        public virtual void Visualize()
        {
        }

        public virtual void Finish()
        {
        }

        public virtual void Cancel()
        {
        }
    }
}
using Heyworks.PocketShooter.Audio;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Lean.Pool;
using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    public class SprintSkillController : SkillController, IMoveSpeedMultiplier
    {
        private SprintSkillInfo Config { get; }

        private GameObject effect;

        public SprintSkillController(OwnedSkill skillModel, SkillSpec spec, LocalCharacter character)
            : base(skillModel, spec, character)
        {
            this.Config = (SprintSkillInfo)skillModel.Info;

            character.CharacterController.AddMoveSpeedMultiplier(this);
        }

        public float SpeedMultiplier => Model.State == SkillState.Active ? Config.SpeedMultiplier : 1f;
    }
}
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills.Configuration;
using UniRx;

namespace Heyworks.PocketShooter.Skills
{
    public class ShockWaveJumpSkillController : SkillController, IFreezer
    {
        private readonly ICastableSkill skill;

        private readonly Jump jump;
        private readonly AoESkill shockWave;

        public bool FreezeMotion => skill.Casting;

        public bool FreezeRotation => skill.Casting;

        public ShockWaveJumpSkillController(OwnedSkill skillModel, ShockWaveJumpSkillSpec spec, LocalCharacter character)
            : base(skillModel, spec, character)
        {
            skill = (ICastableSkill)skillModel.Skill;

            jump = new Jump(character, spec.Angle, spec.Speed);

            var shockWaveJumpSkillConfig = (AoESkillInfo)Model.Info;
            shockWave = new AoESkill(SkillName.ShockWaveJump, Character, shockWaveJumpSkillConfig.AoE);

            character.ModelEvents.SkillCastChanged.Where(
                    e => e.SkillName == SkillName)
                .Subscribe(CastChanged)
                .AddTo(character);

            character.InputController.AddFreezer(this);
        }

        private void CastChanged(SkillCastChangedEvent e)
        {
            if (!e.IsCasting)
            {
                jump.Execute();
                shockWave.Execute();
            }
        }
    }
}

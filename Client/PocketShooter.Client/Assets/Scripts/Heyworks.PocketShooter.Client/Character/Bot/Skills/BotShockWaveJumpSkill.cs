using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills.Configuration;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotShockWaveJumpSkill : BotSkill
    {
        private BotJump jump;
        private BotAoESkill shockWave;

        public BotShockWaveJumpSkill(OwnedSkill skillModel, ShockWaveJumpSkillSpec spec, BotCharacter bot)
            : base(skillModel, spec, bot)
        {
//            jump = new BotJump(bot, spec.Angle, spec.Speed);

            var shockWaveJumpSkillConfig = (AoESkillInfo)Model.Info;
            shockWave = new BotAoESkill(SkillName.ShockWaveJump, bot, shockWaveJumpSkillConfig.AoE);
        }

        protected override void CastChanged(SkillCastChangedEvent e)
        {
            base.CastChanged(e);

            if (!e.IsCasting)
            {
//                jump.Execute();
                shockWave.Execute();
            }
        }
    }
}
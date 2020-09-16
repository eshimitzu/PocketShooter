using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using UniRx;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotShockWaveSkill : BotSkill
    {
        private AoESkillInfo AoESkillConfig => (AoESkillInfo)Model.Info;

        private BotAoESkill shockWave;

        public BotShockWaveSkill(OwnedSkill skillModel, SkillSpec spec, BotCharacter bot)
            : base(skillModel, spec, bot)
        {
            shockWave = new BotAoESkill(SkillName.ShockWave, Bot, AoESkillConfig.AoE);

            bot.ModelEvents.SkillCastChanged.Where(
                    e => e.SkillName == SkillName)
                .Subscribe(CastChanged)
                .AddTo(Bot);
        }

        protected override void CastChanged(SkillCastChangedEvent e)
        {
            base.CastChanged(e);

            if (!e.IsCasting)
            {
                shockWave.Execute();
            }
        }
    }
}
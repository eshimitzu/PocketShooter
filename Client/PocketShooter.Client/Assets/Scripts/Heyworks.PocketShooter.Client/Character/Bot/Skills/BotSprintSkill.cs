using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Skills;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotSprintSkill : BotSkill, IMoveSpeedMultiplier
    {
        public float SpeedMultiplier => Model.State == SkillState.Active ? Config.SpeedMultiplier : 1f;

        private SprintSkillInfo Config { get; }

        public BotSprintSkill(OwnedSkill skillModel, SkillSpec spec, BotCharacter bot)
            : base(skillModel, spec, bot)
        {
            Config = (SprintSkillInfo)skillModel.Info;
            Bot.Movement.AddMoveSpeedMultiplier(this);
        }
    }
}
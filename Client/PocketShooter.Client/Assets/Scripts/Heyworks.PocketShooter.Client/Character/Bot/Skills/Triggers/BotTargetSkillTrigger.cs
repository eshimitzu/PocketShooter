using Heyworks.PocketShooter.Networking.Actors;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Skills.Triggers
{
    public class BotTargetSkillTrigger : BotSimpleSkillTrigger
    {
        private float range;

        public BotTargetSkillTrigger(BotCharacter bot, float range, float damage, float defense, float heal)
            : base(bot, damage, defense, heal)
        {
            this.range = range;
        }

        public override float GetUtility()
        {
            float w = 0;
            EnemyObserver.EnemyTarget currentTarget = bot.Observer.CurrentTarget;
            if (currentTarget != null && currentTarget.IsVisible)
            {
                var distance = Vector3.Distance(currentTarget.Player.Transform.Position, bot.Model.Transform.Position);
                if (distance < range)
                {
                    w = 1;
                }
            }

            return base.GetUtility() * w;
        }
    }
}
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Skills.Triggers
{
    public class BotAOESkillTrigger : BotSimpleSkillTrigger
    {
        private float radius;

        public BotAOESkillTrigger(BotCharacter bot, float radius, float damage, float defense, float heal)
            : base(bot, damage, defense, heal)
        {
            this.radius = radius;
        }

        public override float GetUtility()
        {
            int count = 0;
            foreach (IRemotePlayer target in bot.Observer.VisibleCharacters)
            {
                Vector3 a = target.Transform.Position;
                Vector3 b = bot.Model.Transform.Position;

                var d = Vector3.Distance(a, b);
                if (d < radius)
                {
                    count++;
                }
            }

            return base.GetUtility() + count * 0.5f;
        }
    }
}
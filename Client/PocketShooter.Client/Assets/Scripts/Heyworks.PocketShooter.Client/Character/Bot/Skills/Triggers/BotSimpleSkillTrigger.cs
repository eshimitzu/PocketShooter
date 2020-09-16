using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;

namespace Heyworks.PocketShooter.Character.Bot.Skills.Triggers
{
    public class BotSimpleSkillTrigger : BotSkillTrigger
    {
        private float damage;
        private float defense;
        private float heal;

        public BotSimpleSkillTrigger(BotCharacter bot, float damage, float defense, float heal)
            : base(bot)
        {
            this.damage = damage;
            this.defense = defense;
            this.heal = heal;
        }

        public override float GetUtility()
        {
            HealthComponent health = bot.Model.Health;
            float healthState = 1 - health.Health / health.MaxHealth;
            var underAttack = bot.Observer.DamageInputs.Count * 0.5f;
            var attacking = bot.Observer.AttackableCharacters.Count;

            float total = healthState * heal + underAttack * defense + attacking * damage;
            return total;
        }
    }
}
using Heyworks.PocketShooter.Networking.Actors;

namespace Heyworks.PocketShooter.Character.Bot.Skills.Triggers
{
    public abstract class BotSkillTrigger
    {
        protected BotCharacter bot;

        public abstract float GetUtility();

        public BotSkillTrigger(BotCharacter bot)
        {
            this.bot = bot;
        }
    }
}

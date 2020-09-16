using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Networking.Actors;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    public abstract class BotAction : Action
    {
        protected EnemyObserver Observer { get; private set; } = null;

        protected BotCharacter Bot { get; private set; } = null;

        public override void OnAwake()
        {
            base.OnAwake();

            Observer = gameObject.GetComponent<EnemyObserver>();
            Bot = gameObject.GetComponent<BotCharacter>();
        }

        public override void OnStart()
        {
            base.OnStart();
        }
    }
}
using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Character.Bot.Actions;
using Heyworks.PocketShooter.Skills;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    [TaskCategory("Bot")]
    public class BotUseSkill : BotAction
    {
        [SerializeField]
        private int id = 1;

        [SerializeField]
        private SkillControllerFactory factory;

        private BotSkill skill;

        public override void OnAwake()
        {
            base.OnAwake();
            skill = BotSkillFactory.CreateBotSkill(Bot, id, factory);
//            UnityEngine.Debug.Log($"add skill : {skill?.SkillName}", Bot);
        }

        public override void OnStart()
        {
            base.OnStart();
            skill?.OnStart();
        }

        public override TaskStatus OnUpdate()
        {
            if (skill == null)
            {
                return TaskStatus.Success;
            }

            return skill.OnUpdate();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            skill?.OnEnd();
        }
    }
}
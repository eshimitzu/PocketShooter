using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Character.Bot.Actions;
using Heyworks.PocketShooter.Character.Bot.Skills.Triggers;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Skills;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    [TaskCategory("Bot")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
    public class BotCanUseSkill : BotConditional
    {
        [SerializeField]
        private int id = 1;

        private ClientPlayer player;
        private OwnedSkill skill;
        private BotSkillTrigger skillTrigger;

        public override void OnAwake()
        {
            base.OnAwake();

            player = Bot.Model as ClientPlayer;
            skill = BotSkillFactory.GetSkill(player, id);
            skillTrigger = BotSkillFactory.CreateTrigger(Bot, id);
        }

        public override TaskStatus OnUpdate()
        {
            var utility = skillTrigger.GetUtility();
            this.FriendlyName = $"{skill.Name}:{utility}";
            if (skill.CanUseSkill() && utility > 0.7f || Input.GetKey(KeyCode.Alpha0 + id))
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public override float GetUtility()
        {
            return skillTrigger.GetUtility();
        }
    }
}
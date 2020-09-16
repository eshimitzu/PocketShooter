using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Skills;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.AI;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotJumpSkill : BotSkill
    {
        private BotJump jump;

        public BotJumpSkill(OwnedSkill skillModel, JumpSkillSpec spec, BotCharacter bot)
            : base(skillModel, spec, bot)
        {
//            jump = new BotJump(bot, spec.Angle, spec.Speed);
        }

        public override void OnStart()
        {
            base.OnStart();
//            jump.Execute();
        }

        public override TaskStatus OnUpdate()
        {
//            if (jump.InAir)
//            {
//                return TaskStatus.Running;
//            }

            return TaskStatus.Success;
        }
    }
}
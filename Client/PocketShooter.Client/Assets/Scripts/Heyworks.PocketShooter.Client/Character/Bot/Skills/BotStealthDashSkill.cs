using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills.Configuration;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotStealthDashSkill : BotSkill
    {
        private readonly StealthDashSkillInfo config;
        private bool isDashing;
        private float dashDuration;
        private float dashTime;
        private Vector3 direction;

        public BotStealthDashSkill(OwnedSkill skillModel, StealthDashSkillSpec spec, BotCharacter bot)
            : base(skillModel, spec, bot)
        {
            config = (StealthDashSkillInfo)skillModel.Info;
            dashDuration = config.Length / config.Speed;
        }

        protected override void CastChanged(SkillCastChangedEvent e)
        {
            base.CastChanged(e);

            if (!e.IsCasting)
            {
                isDashing = true;
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            dashTime = 0;

            Vector3 pos = Bot.transform.position;
            Vector3 dir = Bot.transform.forward;
            dir.y = 0;
            dir = dir.normalized;
            Bot.NavMesh.velocity = dir * config.Speed;
        }

        public override TaskStatus OnUpdate()
        {
            if (isDashing)
            {
                dashTime += Time.deltaTime;
            }

            if (dashTime < dashDuration)
            {
                return TaskStatus.Running;
            }

            Bot.NavMesh.velocity = Vector3.zero;

            return base.OnUpdate();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            isDashing = false;
        }
    }
}
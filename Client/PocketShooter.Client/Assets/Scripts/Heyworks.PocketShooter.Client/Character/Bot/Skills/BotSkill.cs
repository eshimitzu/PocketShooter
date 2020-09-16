using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotSkill
    {
        public SkillName SkillName => Model.Name;

        public OwnedSkill Model { get; }

        public SkillSpec Spec { get; }

        protected BotCharacter Bot { get; }

        private bool castable;
        private bool waitForCast;
        private float castDelay;

        public BotSkill(OwnedSkill skillModel, SkillSpec spec, BotCharacter bot)
        {
            Model = skillModel;
            Spec = spec;
            Bot = bot;

            castable = (Model.Skill is CastableSkill);
            if (castable)
            {
                Bot.ModelEvents.SkillCastChanged.Where(
                        e => e.SkillName == SkillName)
                    .Subscribe(CastChanged)
                    .AddTo(Bot);
            }
        }

        protected virtual void CastChanged(SkillCastChangedEvent e)
        {
            if (!e.IsCasting)
            {
                waitForCast = false;
            }
        }

        public virtual void OnStart()
        {
            Bot.UseSkill(Model.Skill.Name);
            waitForCast = castable;
            castDelay = 0;
        }

        public virtual void OnEnd()
        {
        }

        public virtual TaskStatus OnUpdate()
        {
            if (waitForCast)
            {
                castDelay += Time.deltaTime;
                if (castDelay < 0.5f)
                {
                    castDelay += Time.deltaTime;
                }
                else
                {
                    waitForCast = false;
                }
            }

            if (waitForCast)
            {
                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }
    }
}
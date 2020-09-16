using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Skills;
using Heyworks.PocketShooter.Skills;
using Lean.Pool;
using UniRx;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Skills
{
    public class BotGrenadeSkill : BotSkill
    {
        private float timer = 0;
        private float aimingTime = 0;
        private float animationTime = 0;

        private new GrenadeSkillSpec Spec { get; }

        public BotGrenadeSkill(OwnedSkill skillModel, GrenadeSkillSpec spec, BotCharacter bot)
            : base(skillModel, spec, bot)
        {
            Spec = spec;
        }

        public override void OnStart()
        {
            timer = 0;
            aimingTime = Random.Range(0.5f, 3f);
            animationTime = 0;

            Bot.AddCommand(new AimSkillCommandData(Bot.Id, SkillName.Grenade, true));
        }

        public override TaskStatus OnUpdate()
        {
            IRemotePlayer target = Bot.Observer.CurrentTarget?.Player;
            if (target != null)
            {
                Vector3 forward = target.Transform.Position - Bot.transform.position;
                Vector3 tr = Quaternion.LookRotation(forward, Vector3.up).eulerAngles;

                Bot.transform.rotation = Quaternion.Lerp(Bot.transform.rotation, Quaternion.Euler(0, tr.y, tr.z), Time.deltaTime * 1000);
                Bot.EyeCamera.transform.localRotation = Quaternion.Lerp(Bot.EyeCamera.transform.localRotation, Quaternion.Euler(tr.x, 0, 0), Time.deltaTime * 1000);
            }

            if (timer < aimingTime)
            {
                timer += Time.deltaTime;
                return TaskStatus.Running;
            }

            Bot.AddCommand(new AimSkillCommandData(Bot.Id, SkillName.Grenade, false));
            Bot.UseSkill(SkillName);

            if (animationTime < 0.1f)
            {
                animationTime += Time.deltaTime;
                return TaskStatus.Running;
            }

            // Throw
            var config = Bot.Model.GetFirstSkillInfo<GrenadeSkillInfo>(SkillName.Grenade);

            GrenadeProxy grenadeInstance = LeanPool.Spawn(
                Spec.GrenadeProxyPrefab,
                Bot.transform.position + Vector3.up * 0.5f,
                Quaternion.identity,
                null);

            grenadeInstance.Setup(Bot, config.ExplosionRadius, GetGrenadeLayer());

            float aimAngle = Bot.Model.Transform.Pitch;
            Quaternion throwAngle = Quaternion.Euler(Spec.InitialThrowAngle + aimAngle, 0f, 0f);
            Vector3 force = (Bot.transform.rotation * throwAngle) * new Vector3(0, 0, Spec.ThrowPower);
            grenadeInstance.Throw(force);

            return TaskStatus.Success;
        }

        private int GetGrenadeLayer()
        {
            return Bot.IsEnemy ? Spec.EnemyThrowableItemLayer : Spec.AllyThrowableItemLayer;
        }
    }
}
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Utils.Extensions;
using UnityEngine;

namespace Heyworks.PocketShooter.Character.Bot.Actions
{
    [TaskCategory("Bot")]
    public class BotAim : BotAction
    {
        [SerializeField]
        private float turnSpeedMin = 5;

        [SerializeField]
        private float turnSpeedMax = 20;

        private Transform eye;
        private float turnSpeed;

        public override void OnAwake()
        {
            base.OnAwake();
            eye = Bot.EyeCamera.transform;
        }

        public override void OnStart()
        {
            base.OnStart();

            turnSpeed = Random.Range(turnSpeedMin, turnSpeedMax);
        }

        public override TaskStatus OnUpdate()
        {
            IRemotePlayer target = Observer.CurrentTarget?.Player;
            if (target != null)
            {
                Vector3 forward = target.Transform.Position - Bot.transform.position;
                Vector3 tr = Quaternion.LookRotation(forward, Vector3.up).eulerAngles;

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, tr.y, tr.z), Time.deltaTime * turnSpeed);
                eye.localRotation = Quaternion.Lerp(eye.localRotation, Quaternion.Euler(tr.x, 0, 0), Time.deltaTime * turnSpeed);
            }
            else
            {
                eye.localRotation = Quaternion.identity;
            }

            if (target != null)
            {
                if (Observer.IsEnemyInCrosshair)
                {
                    return TaskStatus.Success;
                }
                else
                {
                    return TaskStatus.Running;
                }
            }

            return TaskStatus.Failure;
        }
    }
}
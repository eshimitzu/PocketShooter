using System;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using KinematicCharacterController;
using UnityEngine;

namespace Heyworks.PocketShooter.Networking.Actors
{
    [RequireComponent(typeof(PocketCharacterController))]
    public class LocalCharacter : NetworkCharacter, IFreezer
    {
        [SerializeField]
        private PocketCharacterController characterController;
        [SerializeField]
        private InputController inputController;
        [SerializeField]
        private KinematicCharacterMotor trooperKinematicCharacterMotor;

        private IClientSimulation simulation;

        public bool FreezeMotion => Model.Effects.Stun.IsStunned || Model.Effects.Root.IsRooted;

        public bool FreezeRotation => Model.Effects.Stun.IsStunned || IsDead;

        public event Action<int, float> Damaged;

        public InputController InputController => inputController;

        public PocketCharacterController CharacterController => characterController;

        public new IClientPlayer Model { get; private set; }

        public PlayerInput Input { get; private set; }

        public SkillController FirstSkillController { get; internal set; }

        public SkillController SecondSkillController { get; internal set; }

        public SkillController ThirdSkillController { get; internal set; }

        public SkillController FourthSkillController { get; internal set; }

        public SkillController FifthSkillController { get; internal set; }

        [SuppressMessage(
            "StyleCop.CSharp.OrderingRules",
            "SA1202:ElementsMustBeOrderedByAccess",
            Justification = "Reviewed.")]
        public void Setup(
            IClientPlayer model,
            IClientSimulation simulation,
            ITickEvents tickEvents,
            ICharacterContainer characterContainer,
            float deadHitPower)
        {
            Setup(model, model.Events, simulation, characterContainer, false);
            Model = model;
            this.simulation = simulation;

            characterController.SetupMaxSpeed(model.Info.Speed);
            characterController.AddMoveSpeedMultiplier(new DebugMoveSpeedMultiplier());

            DeadHitPower = deadHitPower;

            InputController.Setup(tickEvents);
            inputController.AddFreezer(this);
        }

        public void AddCommand(IGameCommandData commandData)
        {
            simulation.AddCommand(commandData);
        }

        public void PlaceAtPoint(Vector3 position, Quaternion rotation)
        {
            characterController.SetPosition(position);
            characterController.SetRotation(rotation);
        }

        protected override void HandleDamage(in DamageInfo damage)
        {
            base.HandleDamage(in damage);

            Damaged?.Invoke(damage.AttackerId, damage.Damage);
        }

        protected override void CharacterDeath(KilledServerEvent kse)
        {
            base.CharacterDeath(kse);

            FirstSkillController.Cancel();
            SecondSkillController.Cancel();
            ThirdSkillController.Cancel();
            FourthSkillController.Cancel();
            FifthSkillController.Cancel();

            ActiveRagdoll(kse.ActorId);
        }

        protected override void ActiveRagdoll(EntityId killerId)
        {
            if (trooperKinematicCharacterMotor != null)
            {
                trooperKinematicCharacterMotor.Capsule.enabled = false;
                trooperKinematicCharacterMotor.enabled = false;
            }

            base.ActiveRagdoll(killerId);
        }

        // force position immediately, e.g. for spawn
        internal void ForceLocation(Position position, float yaw)
        {
            transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
            // consider all child components get that value either
        }
    }
}
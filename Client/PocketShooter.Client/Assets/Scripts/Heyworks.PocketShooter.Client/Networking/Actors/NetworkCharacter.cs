using Heyworks.PocketShooter.Camera;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Modules.DamageHints;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Weapons.AimAssistant;
using Microsoft.Extensions.Logging;
using SRF;
using UniRx;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Networking.Actors
{
    public abstract class NetworkCharacter : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<Object, NetworkCharacter>
        {
        }

        [SerializeField]
        private CharacterView characterView;

        [SerializeField]
        private DamageHintController damageHintController;

        [SerializeField]
        private CharacterCommon characterCommon;

        [SerializeField]
        private AimAssistantTarget aimAssistantTarget;

        [Inject]
        private OrbitCamera orbitCamera;

        private int deadLayer;

        private SchedulerTask schedulerMedKitTask;

        public EntityId Id => Model.Id;

        public IPlayer Model { get; private set; }

        public IPlayerEvents ModelEvents { get; private set; }

        public bool IsDead { get; private set; }

        public CharacterCommon CharacterCommon => characterCommon;

        public CharacterView CharacterView => characterView;

        public AimAssistantTarget AimAssistantTarget => aimAssistantTarget;

        public Transform OrbitCamera => orbitCamera.Transform;

        public ICharacterContainer CharacterContainer { get; private set; }

        public SkillVisualizer FirstSkillVisualizer { get; internal set; }

        public SkillVisualizer SecondSkillVisualizer { get; internal set; }

        public SkillVisualizer ThirdSkillVisualizer { get; internal set; }

        public SkillVisualizer FourthSkillVisualizer { get; internal set; }

        public SkillVisualizer FifthSkillVisualizer { get; internal set; }

        public bool IsEnemy { get; private set; }

        protected IClientSimulation Simulation { get; private set; }

        protected float DeadHitPower { get; set; }

        protected StringNumbersCache StringNumbersCache = new StringNumbersCache(0, 200);

        protected DamageHintController DamageHintController => damageHintController;

        protected virtual void Awake()
        {
            deadLayer = LayerMask.NameToLayer("Dead");
        }

        private void OnDestroy()
        {
            if (schedulerMedKitTask != null)
            {
                SchedulerManager.Instance.RemoveSchedulerTask(schedulerMedKitTask);
                schedulerMedKitTask = null;
            }
        }

        public void Reload() =>
            Simulation.AddCommand(new ReloadCommandData(Model.Id));

        public void UseSkill(SkillName skillName)
        {
            GameLog.Debug("Use skill : {skillName}", skillName);

            Simulation.AddCommand(new UseSkillCommandData(Model.Id, skillName));
        }

        protected void Setup(
            IPlayer model,
            IPlayerEvents events,
            IClientSimulation simulation,
            ICharacterContainer characterContainer,
            bool isEnemy)
        {
            Model = model;
            ModelEvents = events;
            this.Simulation = simulation;
            CharacterContainer = characterContainer;
            IsEnemy = isEnemy;

            events.Damaged.Subscribe(HandleDamages).AddTo(this);
            events.Healed.Subscribe(ShowHealHint).AddTo(this);
            events.Killed.Subscribe(CharacterDeath).AddTo(this);
            events.ExpendablesChanged.Subscribe(UpdateExpendables).AddTo(this);

            events.SkillStateChanged.Where(f => f.SkillName == FirstSkillVisualizer.SkillName && f.Activated)
                .Subscribe((f) => { FirstSkillVisualizer.Visualize(); }).AddTo(this);
            events.SkillStateChanged.Where(f => f.SkillName == SecondSkillVisualizer.SkillName && f.Activated)
                .Subscribe((f) => { SecondSkillVisualizer.Visualize(); }).AddTo(this);
            events.SkillStateChanged.Where(f => f.SkillName == ThirdSkillVisualizer.SkillName && f.Activated)
                .Subscribe((f) => { ThirdSkillVisualizer.Visualize(); }).AddTo(this);
            events.SkillStateChanged.Where(f => f.SkillName == FourthSkillVisualizer.SkillName && f.Activated)
               .Subscribe((f) => { FourthSkillVisualizer.Visualize(); }).AddTo(this);
            events.SkillStateChanged.Where(f => f.SkillName == FifthSkillVisualizer.SkillName && f.Activated)
              .Subscribe((f) => { FifthSkillVisualizer.Visualize(); }).AddTo(this);

            events.SkillStateChanged.Where(
                    f => f.SkillName == FirstSkillVisualizer.SkillName && f.Previous == SkillState.Active)
                .Subscribe((f) => { FirstSkillVisualizer.Finish(); }).AddTo(this);
            events.SkillStateChanged.Where(
                    f => f.SkillName == SecondSkillVisualizer.SkillName && f.Previous == SkillState.Active)
                .Subscribe((f) => { SecondSkillVisualizer.Finish(); }).AddTo(this);
            events.SkillStateChanged.Where(
                    f => f.SkillName == ThirdSkillVisualizer.SkillName && f.Previous == SkillState.Active)
                .Subscribe((f) => { ThirdSkillVisualizer.Finish(); }).AddTo(this);
            events.SkillStateChanged.Where(
                f => f.SkillName == FourthSkillVisualizer.SkillName && f.Previous == SkillState.Active)
            .Subscribe((f) => { FourthSkillVisualizer.Finish(); }).AddTo(this);
            events.SkillStateChanged.Where(
                f => f.SkillName == FifthSkillVisualizer.SkillName && f.Previous == SkillState.Active)
            .Subscribe((f) => { FifthSkillVisualizer.Finish(); }).AddTo(this);
        }

        protected virtual void HandleDamages(DamagedServerEvent dse)
        {
            var damages = dse.Damages.Span;
            for (int i = 0; i < damages.Length; i++)
            {
                ref readonly var damage = ref damages[i];
                HandleDamage(in damage);
            }
        }

        protected virtual void HandleDamage(in DamageInfo damage)
        {
            switch (damage.DamageSource.EntityType)
            {
                case EntityType.LifestealSkill:
                    characterView.LifestealEffectController.PlayLifestealed();
                    break;
            }
        }

        protected virtual void UpdateExpendables(EntityId any)
        {
        }

        protected virtual void ActiveRagdoll(EntityId killerID)
        {
            Vector3 hitDirection = Vector3.zero;

            NetworkCharacter killerCharacter = CharacterContainer.GetCharacter(killerID);

            if (killerCharacter != null)
            {
                hitDirection = (transform.position - killerCharacter.transform.position).normalized;
            }

            CharacterCommon.TrooperAvatar.Animator.enabled = false;

            foreach (Rigidbody ragdollBody in characterCommon.TrooperAvatar.RagdollBodies)
            {
                ragdollBody.isKinematic = false;
            }

            foreach (Collider ragdollBodyColider in characterCommon.TrooperAvatar.RagdollBodiesColliders)
            {
                ragdollBodyColider.enabled = true;
            }

            characterCommon.TrooperAvatar.RagdollBodies[0].AddForce(hitDirection * DeadHitPower, ForceMode.Impulse);

            CharacterView.ThrowWeaponView();
        }

        protected virtual void CharacterDeath(KilledServerEvent kse)
        {
            IsDead = true;

            gameObject.SetLayerRecursive(deadLayer);

            FirstSkillVisualizer.Cancel();
            SecondSkillVisualizer.Cancel();
            ThirdSkillVisualizer.Cancel();
            FourthSkillVisualizer.Cancel();
            FifthSkillVisualizer.Cancel();

            characterView.StopEffects();

            switch (kse.DeathType)
            {
                case DeathType.RegenerationOnKill:
                    characterView.LifestealEffectController.PlayLifestealed();
                    break;
            }
        }

        private void ShowHealHint(HealingServerEvent healEvent)
        {
            var heals = healEvent.Heals.Span;
            for (var i = 0; i < heals.Length; i++)
            {
                ref readonly var heal = ref heals[i];
                switch (heal.Type)
                {
                    case HealType.Lifesteal:
                    case HealType.RegenerationOnKill:
                        characterView.LifestealEffectController.PlayLifesteal();
                        break;
                }

                Vector3 toCameraDirection = (orbitCamera.transform.position - transform.position).normalized;

                DamageHintController.ShowHealHint(
                    CharacterCommon.HeadHintTransform.position,
                    StringNumbersCache.GetString((int)healEvent.Heals[i].Amount),
                    toCameraDirection);
            }
        }
    }
}
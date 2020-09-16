using System.Linq;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Character.Bot;
using Heyworks.PocketShooter.Core.SchedulerManager;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Entities.Weapon;
using Heyworks.PocketShooter.Realtime.Simulation;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Heyworks.PocketShooter.Networking.Actors
{
    public class BotCharacter : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<Object, BotCharacter>
        {
        }

        [SerializeField]
        private UnityEngine.Camera eyeCamera = null;

        [Inject]
        private RealtimeRunBehavior runBehavior;

        private PlayerInfo botInfo;

        public NavMeshAgent NavMesh { get; private set; }

        public EnemyObserver Observer { get; private set; }

        public MovementModifiers Movement { get; } = new MovementModifiers();

        public IClientSimulation Simulation { get; private set; }

        public IClientPlayer Model { get; private set; }

        public ClientPlayer LocalPlayer => runBehavior._RoomController.LocalPlayerSimulation.Game.LocalPlayer;

        public IPlayerEvents ModelEvents => Model.Events;

        public EntityId Id => Model.Id;

        public bool IsEnemy { get; private set; }

        public UnityEngine.Camera EyeCamera => eyeCamera;

        public void PlaceAtPoint(Vector3 position, Quaternion rotation)
        {
            NavMesh.enabled = false;
            transform.position = position;
            transform.rotation = rotation;
            NavMesh.enabled = true;
        }

        public void Setup(PlayerInfo botInfo, IClientPlayer model, ITickProvider tickProvider, IClientSimulation simulation, ITickEvents events, bool isEnemy)
        {
            this.botInfo = botInfo;
            this.Simulation = simulation;
            this.Model = model;
            this.IsEnemy = isEnemy;

            model.Events.Killed.Subscribe(DeathActions).AddTo(this);
            model.Events.AmmoChanged.Subscribe(UpdateAmmo).AddTo(this);
            events.SimulationTick.Subscribe(OnTick).AddTo(this);

            NavMesh.speed = Model.Info.Speed * Movement.SpeedMultiplier;
        }

        public void AddCommand(IGameCommandData gameCommand)
        {
            Simulation.AddCommand(gameCommand);
        }

        public void AddCommand(IServiceCommandData serviceCommand)
        {
            Simulation.AddCommand(serviceCommand);
        }

        public void UseSkill(SkillName skillName)
        {
//            Debug.Log($"UseSkill {skillName}");
            Simulation.AddCommand(new UseSkillCommandData(Model.Id, skillName));
        }

        private void Awake()
        {
            NavMesh = GetComponent<NavMeshAgent>();
            Observer = GetComponent<EnemyObserver>();
        }

        private void Update()
        {
            NavMesh.speed = Model.Info.Speed * Movement.SpeedMultiplier;
        }

        private void DeathActions(KilledServerEvent kse)
        {
            NavMesh.enabled = false;
            SchedulerManager.Instance.CallActionWithDelay(
                this,
                () =>
                {
                    TrooperClass[] availableClasses = botInfo.Troopers.Select(item => item.Class).ToArray();
                    int randomClassIndex = Random.Range(0, availableClasses.Length);
                    var trooper = availableClasses[randomClassIndex];

                    this.AddCommand(new SpawnBotTrooperCommandData(botInfo.Id, trooper));
                },
                5f);
        }

        private void UpdateAmmo(int ammoValue)
        {
            switch (Model.CurrentWeapon)
            {
                // NOTE: can improve if no enemies near and low ammo
                case IConsumableWeapon ammo when ammo.AmmoInClip == 0:
                    AddCommand(new ReloadCommandData(Model.Id));
                    break;
            }
        }

        private void OnTick(TickEvent e)
        {
            if (Model.IsAlive)
            {
                var pos = transform.position;
                var commandData = new MoveCommandData(
                    Model.Id,
                    new FpsTransformComponent(
                    pos,
                    transform.rotation.eulerAngles.y,
                    eyeCamera.transform.rotation.eulerAngles.x));

                this.AddCommand(commandData);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Camera;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Core;
using Heyworks.PocketShooter.Networking;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Realtime.Simulation;
using Heyworks.PocketShooter.Skills;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Utils.Unity.EditorTools.Attributes;
using Heyworks.PocketShooter.Weapons;
using Heyworks.PocketShooter.Weapons.AimAssistant;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace Heyworks.PocketShooter.Character
{
    public class TrooperCreator : MonoBehaviour
    {
        [SerializeField]
        private TroopersConfig troopersConfig;

        [SerializeField]
        private WeaponsConfig weaponsConfig;

        [SerializeField]
        private SkillControllerFactory skillFactory;

        [SerializeField]
        private RuntimeAnimatorController bareHandsAnimationController;

        [SerializeField]
        [Layer]
        private int enemyLayer;

        [SerializeField]
        [Layer]
        private int allyLayer;

        [SerializeField]
        [Layer]
        private int playerLayer;

        [SerializeField]
        private GameObject dummyTrooperPrefab;

        [Inject]
        private NetworkCharacter.Factory networkCharacterFactory;
        [Inject]
        private DummyCharacter.Factory dummyPlayerFactory;
        [Inject]
        private BotCharacter.Factory botCharacterFactory;

        [Inject]
        private OrbitCamera orbitCamera;

        [Inject]
        private MapSceneManager mapSceneManager;

        private GameObject trooperGameObjectsContainer;

        private void Start()
        {
            trooperGameObjectsContainer = new GameObject("TroopersContainer");
            mapSceneManager.MapSceneLoaded += Map_Loaded;
        }

        private void OnDisable()
        {
            mapSceneManager.MapSceneLoaded -= Map_Loaded;
        }

        private void Map_Loaded()
        {
            mapSceneManager.TransferGameObjectToActiveScene(trooperGameObjectsContainer);
        }

        public LocalCharacter CreateLocalTrooperWithType(
            IClientPlayer model,
            IClientSimulation simulation,
            ITickEvents tickEvents,
            ICharacterContainer characterContainer)
        {
            if (trooperGameObjectsContainer == null)
            {
                trooperGameObjectsContainer = new GameObject("TroopersContainer");
            }

            TrooperConfig trooperConfig = troopersConfig.GetTrooperWithClass(model.TrooperClass);
            GameObject localCharacterObject = networkCharacterFactory.Create(troopersConfig.LocalCharacter).gameObject;
            localCharacterObject.transform.SetParent(trooperGameObjectsContainer.transform);

            LocalCharacter localCharacter = localCharacterObject.GetComponent<LocalCharacter>();

            TrooperAvatar trooperAvatar = Instantiate(trooperConfig.TrooperAvatar).GetComponent<TrooperAvatar>();

            localCharacter.CharacterCommon.Setup(trooperAvatar);

            localCharacter.Setup(model, simulation, tickEvents, characterContainer, troopersConfig.DeadHitPower);
            var weaponName = model.CurrentWeapon.Name;
            WeaponViewConfig weaponConfig = weaponsConfig.GetWeaponByName(weaponName);
            localCharacter.CharacterView.SetupWeaponView(weaponName, trooperAvatar);

            WeaponRaycaster weaponRaycaster = Instantiate(weaponConfig.WeaponRaycaster, localCharacterObject.transform).GetComponent<WeaponRaycaster>();
            weaponRaycaster.Setup(localCharacter.CharacterCommon.ShotOriginTransfrom);

            WeaponFireVisualiser weaponFireVisualiser = Instantiate(weaponConfig.WeaponFireVisualiser, localCharacter.transform).GetComponent<WeaponFireVisualiser>();
            weaponFireVisualiser.Setup(characterContainer, weaponRaycaster, localCharacter.CharacterView, model.Events);

            WeaponController weaponController = Instantiate(weaponConfig.WeaponController, localCharacterObject.transform).GetComponent<WeaponController>();
            weaponController.Setup(model.CurrentWeapon, simulation, tickEvents, localCharacter, weaponRaycaster, weaponFireVisualiser);

            AimAssistant aimAssistant = localCharacter.GetComponent<AimAssistant>();
            aimAssistant.Setup(model.CurrentWeapon.Info, localCharacter);

            localCharacter.CharacterCommon.AnimationController.Setup(localCharacter.GetComponentInChildren<PocketCharacterController>(), trooperAvatar.Animator);

            localCharacter.GetComponentInChildren<BodyAimRotationHelper>().Setup(weaponName, orbitCamera.Transform, trooperAvatar.Animator);

            var clientPlayer = (ClientPlayer)model;
            localCharacter.FirstSkillVisualizer = skillFactory.CreateSkillVisualizer(clientPlayer.Skill1.Skill, localCharacter);
            localCharacter.SecondSkillVisualizer = skillFactory.CreateSkillVisualizer(clientPlayer.Skill2.Skill, localCharacter);
            localCharacter.ThirdSkillVisualizer = skillFactory.CreateSkillVisualizer(clientPlayer.Skill3.Skill, localCharacter);
            localCharacter.FourthSkillVisualizer = skillFactory.CreateSkillVisualizer(clientPlayer.Skill4.Skill, localCharacter);
            localCharacter.FifthSkillVisualizer = skillFactory.CreateSkillVisualizer(clientPlayer.Skill5.Skill, localCharacter);

            localCharacter.FirstSkillController = skillFactory.CreateLocalSkillController(clientPlayer.Skill1, localCharacter);
            localCharacter.SecondSkillController = skillFactory.CreateLocalSkillController(clientPlayer.Skill2, localCharacter);
            localCharacter.ThirdSkillController = skillFactory.CreateLocalSkillController(clientPlayer.Skill3, localCharacter);
            localCharacter.FourthSkillController = skillFactory.CreateLocalSkillController(clientPlayer.Skill4, localCharacter);
            localCharacter.FifthSkillController = skillFactory.CreateLocalSkillController(clientPlayer.Skill5, localCharacter);

            ref readonly var t = ref model.Transform;           

            localCharacter.ForceLocation(t.Position, t.Yaw);
            
            localCharacter.PlaceAtPoint(
                t.Position,
                Quaternion.Euler(0, t.Yaw, 0));

            localCharacterObject.RunOnChildrenRecursive(go => go.layer = playerLayer);
            localCharacterObject.SetActive(true);

            return localCharacter;
        }

        public RemoteCharacter CreateRemoteTrooperWithType(
            TrooperClass trooperClass,
            WeaponName weaponName,
            bool isEnemy,
            EntityId id,
            IRemotePlayer model,
            ITickProvider tickProvider,
            IClientSimulation simulation,
            ICharacterContainer characterContainer)
        {
            if (trooperGameObjectsContainer == null)
            {
                trooperGameObjectsContainer = new GameObject("TroopersContainer");
            }

            TrooperConfig trooperConfig = troopersConfig.GetTrooperWithClass(trooperClass);
            GameObject remoteCharacterObject = networkCharacterFactory.Create(troopersConfig.RemoteCharacter).gameObject;
            remoteCharacterObject.transform.SetParent(trooperGameObjectsContainer.transform);
            RemoteCharacter remoteCharacter = remoteCharacterObject.GetComponent<RemoteCharacter>();

            TrooperAvatar trooperAvatar = Instantiate(trooperConfig.TrooperAvatar).GetComponent<TrooperAvatar>();
            trooperAvatar.Setup(troopersConfig.TrooperMaterialConfig, isEnemy);

            remoteCharacter.CharacterCommon.Setup(trooperAvatar);

            remoteCharacter.RemoteCharacterView.SetupWeaponView(weaponName, trooperAvatar);
            remoteCharacter.RemoteCharacterView.SetupStatusBar(model, isEnemy);

            WeaponViewConfig weaponConfig = weaponsConfig.GetWeaponByName(weaponName);

            WeaponRaycaster weaponRaycaster = Instantiate(weaponConfig.WeaponRaycaster, remoteCharacterObject.transform).GetComponent<WeaponRaycaster>();
            weaponRaycaster.Setup(remoteCharacter.CharacterCommon.ShotOriginTransfrom);

            remoteCharacter.Setup(id, model, isEnemy, tickProvider, simulation, characterContainer, troopersConfig.DeadHitPower, weaponRaycaster);

            WeaponFireVisualiser weaponFireVisualiser = Instantiate(weaponConfig.WeaponFireVisualiser, remoteCharacterObject.transform).GetComponent<WeaponFireVisualiser>();
            weaponFireVisualiser.Setup(characterContainer, weaponRaycaster, remoteCharacter.CharacterView, model.Events, model, remoteCharacter.OrbitFollowTransform);

            remoteCharacter.CharacterCommon.AnimationController.Setup(remoteCharacter.GetComponentInChildren<RemotePlayerSpeedProvider>(), trooperAvatar.Animator);

            remoteCharacter.GetComponentInChildren<BodyAimRotationHelper>().Setup(weaponName, remoteCharacter.OrbitFollowTransform.transform, trooperAvatar.Animator);

            remoteCharacter.FirstSkillVisualizer = skillFactory.CreateSkillVisualizer(model.Skill1, remoteCharacter);
            remoteCharacter.SecondSkillVisualizer = skillFactory.CreateSkillVisualizer(model.Skill2, remoteCharacter);
            remoteCharacter.ThirdSkillVisualizer = skillFactory.CreateSkillVisualizer(model.Skill3, remoteCharacter);
            remoteCharacter.FourthSkillVisualizer = skillFactory.CreateSkillVisualizer(model.Skill4, remoteCharacter);
            remoteCharacter.FifthSkillVisualizer = skillFactory.CreateSkillVisualizer(model.Skill5, remoteCharacter);

            remoteCharacter.transform.SetPositionAndRotation(
                model.Transform.Position,
                Quaternion.Euler(0, model.Transform.Yaw, 0));

            remoteCharacterObject.RunOnChildrenRecursive(go => go.layer = isEnemy ? enemyLayer : allyLayer);
            remoteCharacterObject.SetActive(true);

            return remoteCharacter;
        }

        // TODO: a.dezhurko move from trooper creator Bot Character is NOT a character.
        public BotCharacter CreateBotTrooperWithType(
            PlayerInfo botInfo,
            TrooperClass trooperClass,
            IClientPlayer model,
            ITickProvider tickProvider,
            IClientSimulation simulation,
            ITickEvents tickEvents,
            bool isEnemy)
        {
            if (trooperGameObjectsContainer == null)
            {
                trooperGameObjectsContainer = new GameObject("TroopersContainer");
            }

            var trooperConfig = troopersConfig.GetTrooperWithClass(trooperClass);
            var trooperObject = botCharacterFactory.Create(trooperConfig.BotCharacter).gameObject;
            trooperObject.transform.SetParent(trooperGameObjectsContainer.transform);

            var bot = trooperObject.GetComponent<BotCharacter>();
            bot.Setup(botInfo, model, tickProvider, simulation, tickEvents, isEnemy);

            bot.PlaceAtPoint(
                model.Transform.Position,
                Quaternion.Euler(0, model.Transform.Yaw, 0));

            trooperObject.SetActive(true);

            return bot;
        }

        public GameObject CreateDummyTrooper(TrooperClass trooperClass)
        {
            Dictionary<TrooperClass, WeaponName> weaponMap = new Dictionary<TrooperClass, WeaponName>()
            {
                { TrooperClass.Rambo, WeaponName.M16A4 },
                { TrooperClass.Sniper, WeaponName.SVD },
                { TrooperClass.Scout, WeaponName.Remington },
                { TrooperClass.Spy, WeaponName.Knife },
                { TrooperClass.Rock, WeaponName.Minigun },
                { TrooperClass.Neo, WeaponName.Katana },
                { TrooperClass.Norris, WeaponName.SawedOff },
                { TrooperClass.Statham, WeaponName.Barret },
            };
            return CreateDummyTrooper(trooperClass, weaponMap[trooperClass]);
        }

        public GameObject CreateDummyTrooper(TrooperClass trooperClass, WeaponName? weaponIdentifer)
        {
            TrooperConfig trooperConfig = troopersConfig.GetTrooperWithClass(trooperClass);

            GameObject trooperObject = dummyPlayerFactory.Create(dummyTrooperPrefab).gameObject;
            DummyCharacter dummyCharacter = trooperObject.GetComponent<DummyCharacter>();

            TrooperAvatar trooperAvatar = Instantiate(trooperConfig.TrooperAvatar).GetComponent<TrooperAvatar>();
            trooperAvatar.FootstepsAudioManager.enabled = false;
            dummyCharacter.CharacterCommon.Setup(trooperAvatar);

            dummyCharacter.CharacterCommon.AnimationController.Setup(
                dummyCharacter.RemotePlayerSpeedProvider,
                trooperAvatar.Animator);

            CharacterView characterView = trooperObject.GetComponent<CharacterView>();

            if (weaponIdentifer.HasValue)
            {
                characterView.SetupWeaponView(weaponIdentifer.Value, trooperAvatar);
            }
            else
            {
                trooperAvatar.Animator.runtimeAnimatorController = bareHandsAnimationController;
            }

            dummyCharacter.CharacterCommon.AnimationController.Idle();

            trooperObject.RunOnChildrenRecursive(go => go.layer = playerLayer);
            trooperObject.SetActive(true);

            var mrs = trooperObject.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in mrs)
            {
                r.lightProbeUsage = LightProbeUsage.Off;
            }

            return trooperObject;
        }
    }
}
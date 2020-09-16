using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Camera;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Modules.GameEnvironment;
using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Weapons;
using UnityEngine;
using Zenject;
using static UnityEngine.UI.Dropdown;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class OfflineTrooperCreator : MonoBehaviour
    {
        [Inject]
        private OfflineLocalCharacter.Factory networkCharacterFactory;

        [Inject]
        private OrbitCamera orbitCamera;

        public WeaponView WeaponView { get; set; }

        public OfflineLocalCharacter LocalCharacter => localCharacter;

        [SerializeField]
        private TroopersConfig troopersConfig;

        [SerializeField]
        private OfflineLocalCharacter offlineLocalCharacter;

        [SerializeField]
        private WeaponsConfig weaponsConfig;

        private GameObject trooperGameObjectsContainer;
        private OfflineLocalCharacter localCharacter;
        private WeaponView.Factory weaponViewFactory;

        [Inject]
        [SuppressMessage(
            "StyleCop.CSharp.OrderingRules",
            "SA1202:ElementsMustBeOrderedByAccess",
            Justification = "Reviewed.")]
        public void Init(WeaponView.Factory weaponFactory)
        {
            AssertUtils.NotNull(weaponFactory, nameof(weaponFactory));

            weaponViewFactory = weaponFactory;
        }

        public List<OptionData> GetTrooperClasses()
        {
            List<OptionData> trooperClasses = new List<OptionData>();

            foreach (TrooperConfig trooperConfig in troopersConfig.Troopers)
            {
                OptionData trooperData = new OptionData();
                trooperData.text = trooperConfig.TrooperClass.ToString();
                trooperClasses.Add(trooperData);
            }

            return trooperClasses;
        }

        public List<OptionData> GetWeaponNames()
        {
            List<OptionData> weaponNames = new List<OptionData>();

            foreach (WeaponViewConfig weaponConfig in weaponsConfig.WeaponList)
            {
                OptionData weaponData = new OptionData();
                weaponData.text = weaponConfig.Name.ToString();
                weaponNames.Add(weaponData);
            }

            return weaponNames;
        }

        public void CreateOfflineTrooperWithType(OfflineClientPlayer model, AnimationUtilityPresenter animUtilityPresenter)
        {
            if (trooperGameObjectsContainer != null)
            {
                GameObject.Destroy(trooperGameObjectsContainer);
            }

            trooperGameObjectsContainer = new GameObject("TroopersContainer");

            TrooperConfig trooperConfig = troopersConfig.GetTrooperWithClass(model.TrooperClass);
            GameObject localCharacterObject = networkCharacterFactory.Create(offlineLocalCharacter).gameObject;
            localCharacterObject.transform.parent = trooperGameObjectsContainer.transform;
            localCharacterObject.transform.position = Vector3.zero;

            localCharacter = localCharacterObject.GetComponent<OfflineLocalCharacter>();

            TrooperAvatar trooperAvatar = Instantiate(trooperConfig.TrooperAvatar).GetComponent<TrooperAvatar>();
            trooperAvatar.Setup(troopersConfig.TrooperMaterialConfig, false);

            localCharacter.CharacterCommon.Setup(trooperAvatar);
            localCharacter.CharacterController.SetupMaxSpeed(model.MaxKinematicSpeed);

            SpawnerTeamPoints spawnerPoints = (SpawnerTeamPoints)FindObjectOfType(typeof(SpawnerTeamPoints));

            Transform spawnPoint = spawnerPoints.SpawnPoints[0];
            localCharacter.ForcePosition(spawnPoint);

            AnimationController animationController = localCharacter.CharacterCommon.AnimationController;
            PocketCharacterController characterController = localCharacter.GetComponentInChildren<PocketCharacterController>();
            characterController.AddMoveSpeedMultiplier(new DebugMoveSpeedMultiplier());
            animationController.Setup(characterController, trooperAvatar.Animator);

            var weaponName = model.WeaponName;
            WeaponViewConfig weaponConfig = weaponsConfig.GetWeaponByName(weaponName);
            SetupWeaponView(weaponConfig, trooperAvatar);
            WeaponRaycaster weaponRaycaster = Instantiate(weaponConfig.WeaponRaycaster, localCharacterObject.transform).GetComponent<WeaponRaycaster>();
            weaponRaycaster.Setup(localCharacter.CharacterCommon.ShotOriginTransfrom);

            InputController inputController = localCharacterObject.GetComponent<InputController>();
            animUtilityPresenter.SetupWhenNewTrooperCreated(animationController, inputController);
        }

        private void SetupWeaponView(WeaponViewConfig weaponViewConfig, TrooperAvatar trooperAvatar)
        {
            WeaponView = weaponViewFactory.Create(weaponViewConfig.View);
            WeaponView.transform.SetParent(trooperAvatar.WeaponViewParent, false);
            trooperAvatar.Animator.runtimeAnimatorController = weaponViewConfig.CharacterAnimatorController;
        }
    }
}
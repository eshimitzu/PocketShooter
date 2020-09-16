using System;
using System.Collections.Generic;
using System.Reflection;
using Heyworks.PocketShooter.AnimationUtility;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Core;
using UnityEngine;
using Zenject;
using static UnityEngine.UI.Dropdown;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class AnimationUtilityPresenter : MonoBehaviour
    {
        private const float SpeedSliderMaxValue = 1f;
        private const float SpeedSliderMidValue = 0.5f;
        private const float DefaultMaxKinematicSpeed = 3f;

        [Inject]
        private OfflineTrooperCreator trooperCreator;

        [SerializeField]
        private MapSceneManager mapSceneManager;

        [SerializeField]
        private TrooperAndSceneSelectionView trooperAndSceneSelectionView;
        [SerializeField]
        private AnimationControlsView animationControlsView;
        [SerializeField]
        private SpeedControlsView speedControlsView;

        private AnimationController animationController;
        private InputController inputController;
        private MapNames map;
        private TrooperClass trooperClass = TrooperClass.Rambo;
        private WeaponName trooperWeapon = WeaponName.M16A4;
        private float maxKinematicSpeed = DefaultMaxKinematicSpeed;

        private List<KeyValuePair<AnimationUtilityButtonAttribute, MethodInfo>> animatorAttributesAndMethods =
            new List<KeyValuePair<AnimationUtilityButtonAttribute, MethodInfo>>();

        public void SetupWhenNewTrooperCreated(AnimationController animationController, InputController inputController)
        {
            this.animationController = animationController;
            this.inputController = inputController;
        }

        private void Awake()
        {
            GetAllAnimationParameters();

            animationControlsView.CreateAnimationControlUI(animatorAttributesAndMethods);
        }

        private void Start()
        {
            SetupTrooperSelectionView();

            AddEventHandlers();
        }

        private void GetAllAnimationParameters()
        {
            Type animationController = typeof(AnimationController);
            MethodInfo[] animationControllerMethods = animationController.GetMethods();

            foreach (MethodInfo methodInfoData in animationControllerMethods)
            {
                Attribute animationControllerMethodAttribute = Attribute.GetCustomAttribute(methodInfoData, typeof(AnimationUtilityButtonAttribute));
                if (animationControllerMethodAttribute == null)
                {
                    continue;
                }

                if (animationControllerMethodAttribute is AnimationUtilityButtonAttribute animationAttribute)
                {
                    AnimationUtilityButtonAttribute buttonAttribute = animationAttribute;
                    MethodInfo method = methodInfoData;

                    animatorAttributesAndMethods.Add(new KeyValuePair<AnimationUtilityButtonAttribute, MethodInfo>(buttonAttribute, method));
                }
            }
        }

        private void SetupTrooperSelectionView()
        {
            trooperAndSceneSelectionView.SetupMapDropDown(GetMapNames());
            trooperAndSceneSelectionView.SetupTrooperClassDropDown(trooperCreator.GetTrooperClasses());
            trooperAndSceneSelectionView.SetupTrooperWeaponDropDown(trooperCreator.GetWeaponNames());
            trooperAndSceneSelectionView.InitializeMaxKinematicSpeedSlider(DefaultMaxKinematicSpeed);
        }

        private List<OptionData> GetMapNames()
        {
            List<OptionData> mapNames = new List<OptionData>();

            foreach (MapNames mapName in (MapNames[])Enum.GetValues(typeof(MapNames)))
            {
                OptionData mapData = new OptionData();
                mapData.text = mapName.ToString();
                mapNames.Add(mapData);
            }

            return mapNames;
        }

        private void AddEventHandlers()
        {
            trooperAndSceneSelectionView.OnMapChanged += AnimationUtilityView_OnMapChanged;
            trooperAndSceneSelectionView.OnTrooperClassChanged += AnimationUtilityView_OnTrooperClassChanged;
            trooperAndSceneSelectionView.OnTrooperWeaponChanged += AnimationUtilityView_OnTrooperWeaponChanged;
            trooperAndSceneSelectionView.OnMaxKinematicSpeedChanged += AnimationUtilityView_OnMaxKinematicSpeedChanged;
            trooperAndSceneSelectionView.OnCreateTrooperClicked += AnimationUtilityView_OnCreateTrooperClicked;
            speedControlsView.OnCurrentMoveSpeedChanged += AnimationUtilityView_OnSpeedChanged;
            speedControlsView.OnRunToggleChanged += AnimationUtilityView_OnRunToggleChanged;
            animationControlsView.OnShootAreaClicked += AnimationUtilityView_OnShootAreaClicked;

            animationControlsView.OnAnimationControllerButtonClicked += AnimationControllerButtonClicked;
            animationControlsView.OnAnimationControllerToggleClicked += AnimationControllerToggleClicked;

            mapSceneManager.MapSceneLoaded += MapSceneManager_MapSceneLoaded;
        }

        private void AnimationUtilityView_OnMapChanged(MapNames map)
        {
            this.map = map;
        }

        private void AnimationUtilityView_OnTrooperClassChanged(TrooperClass trooperClass)
        {
            this.trooperClass = trooperClass;
        }

        private void AnimationUtilityView_OnTrooperWeaponChanged(WeaponName trooperWeapon)
        {
            this.trooperWeapon = trooperWeapon;
        }

        private void AnimationUtilityView_OnMaxKinematicSpeedChanged(float maxKinematicSpeed)
        {
            this.maxKinematicSpeed = maxKinematicSpeed;
        }

        private void AnimationUtilityView_OnCreateTrooperClicked()
        {
            mapSceneManager.LoadMapScene(map);
        }

        private void EnableTrooperAnimationControls()
        {
            animationControlsView.SetControlsActive(true);
        }

        private void EnableTrooperSpeedControls()
        {
            speedControlsView.SetControlsActive(true);
        }

        private void ResetSpeedControls()
        {
            inputController.SetEditorInputSpeedCoeficient(SpeedSliderMidValue);
            speedControlsView.ResetSpeedControls(SpeedSliderMaxValue, SpeedSliderMidValue);
        }

        private void AnimationUtilityView_OnSpeedChanged(float speed)
        {
            inputController.SetEditorInputSpeedCoeficient(speed);
        }

        private void AnimationUtilityView_OnRunToggleChanged(bool isActive)
        {
            inputController.SetEditorInputSpeedCoeficient(isActive ? SpeedSliderMaxValue : SpeedSliderMidValue);
        }

        private void AnimationUtilityView_OnShootAreaClicked()
        {
            animationController.Shoot();
        }

        private void AnimationControllerButtonClicked(string animationName)
        {
            animationController.Invoke(animationName, 0f);
        }

        private void AnimationControllerToggleClicked(string animationName, bool isActive)
        {
            MethodInfo method = animatorAttributesAndMethods.Find(x => x.Value.Name == animationName).Value;
            method.Invoke(animationController, new object[] { isActive });
        }

        private void MapSceneManager_MapSceneLoaded()
        {
            OfflineClientPlayer clientPlayer = new OfflineClientPlayer(trooperClass, trooperWeapon, maxKinematicSpeed);

            trooperCreator.CreateOfflineTrooperWithType(clientPlayer, this);
            EnableTrooperAnimationControls();
            EnableTrooperSpeedControls();
            ResetSpeedControls();
            animationControlsView.ResetToggles();
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Dropdown;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class TrooperAndSceneSelectionView : MonoBehaviour
    {
        public Action<MapNames> OnMapChanged;
        public Action<TrooperClass> OnTrooperClassChanged;
        public Action<WeaponName> OnTrooperWeaponChanged;
        public Action<float> OnMaxKinematicSpeedChanged;
        public Action OnCreateTrooperClicked;

        [SerializeField]
        private Dropdown mapSelector;
        [SerializeField]
        private Dropdown trooperClassSelector;
        [SerializeField]
        private Dropdown trooperWeaponSelector;
        [SerializeField]
        private Slider maxKinematicSpeedSlider;
        [SerializeField]
        private Text maxKinematicSpeedText;
        [SerializeField]
        private Button createTrooperButton;

        public void SetupMapDropDown(List<OptionData> maps)
        {
            mapSelector.options = maps;
        }

        public void SetupTrooperClassDropDown(List<OptionData> trooperClasses)
        {
            trooperClassSelector.options = trooperClasses;
        }

        public void SetupTrooperWeaponDropDown(List<OptionData> trooperWeapons)
        {
            trooperWeaponSelector.options = trooperWeapons;
        }

        public void InitializeMaxKinematicSpeedSlider(float defaultMaxKinematicSpeed)
        {
            maxKinematicSpeedSlider.value = defaultMaxKinematicSpeed;
        }

        private void OnEnable()
        {
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            mapSelector.onValueChanged.AddListener(MapSelector_OnValueChanged);
            trooperClassSelector.onValueChanged.AddListener(TrooperClassSelector_OnValueChanged);
            trooperWeaponSelector.onValueChanged.AddListener(TrooperWeaponSelector_OnValueChanged);
            maxKinematicSpeedSlider.onValueChanged.AddListener(MaxKinematicMoveSpeedSlider_OnValueChanged);
            createTrooperButton.onClick.AddListener(CreateTrooperClickedHandler);
        }

        private void MapSelector_OnValueChanged(int i)
        {
            Enum.TryParse(mapSelector.options[i].text, out MapNames mapNamesEnumValue);
            OnMapChanged?.Invoke(mapNamesEnumValue);
        }

        private void TrooperClassSelector_OnValueChanged(int i)
        {
            Enum.TryParse(trooperClassSelector.options[i].text, out TrooperClass trooperClassEnumValue);
            OnTrooperClassChanged?.Invoke(trooperClassEnumValue);
        }

        private void TrooperWeaponSelector_OnValueChanged(int i)
        {
            Enum.TryParse(trooperWeaponSelector.options[i].text, out WeaponName trooperWeaponEnumValue);
            OnTrooperWeaponChanged?.Invoke(trooperWeaponEnumValue);
        }

        private void MaxKinematicMoveSpeedSlider_OnValueChanged(float maxSpeedValue)
        {
            OnMaxKinematicSpeedChanged?.Invoke(maxSpeedValue);
            maxKinematicSpeedText.text = string.Format("{0:f2}", maxSpeedValue);
        }

        private void CreateTrooperClickedHandler()
        {
            OnCreateTrooperClicked?.Invoke();
        }
    }
}
using Heyworks.PocketShooter.AnimationUtility;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class AnimationControlsView : MonoBehaviour
    {
        public Action OnShootAreaClicked;

        public Action<string> OnAnimationControllerButtonClicked;
        public Action<string, bool> OnAnimationControllerToggleClicked;

        [SerializeField]
        private ShootAreaClickHandler shootArea;

        [SerializeField]
        private GameObject buttonPrefab;
        [SerializeField]
        private GameObject togglePrefab;

        [SerializeField]
        private Transform animatorControlButtonsParent;
        [SerializeField]
        private Transform animatorControlTogglesParent;

        private List<GameObject> toggles = new List<GameObject>();
        private List<GameObject> buttons = new List<GameObject>();

        public void SetControlsActive(bool activeSelf)
        {
            shootArea.gameObject.SetActive(activeSelf);
            foreach (GameObject button in buttons)
            {
                button.SetActive(activeSelf);
            }

            foreach (GameObject toggle in toggles)
            {
                toggle.SetActive(activeSelf);
            }
        }

        public void ResetToggles()
        {
            foreach (GameObject toggle in toggles)
            {
                toggle.GetComponent<Toggle>().isOn = false;
            }
        }

        public void CreateAnimationControlUI(List<KeyValuePair<AnimationUtilityButtonAttribute, MethodInfo>> animatorAttributesAndMethods)
        {
            foreach (KeyValuePair<AnimationUtilityButtonAttribute, MethodInfo> attributeAndMethod in animatorAttributesAndMethods)
            {
                if (attributeAndMethod.Key.IsTrigger)
                {
                    CreateUIButtons(attributeAndMethod);
                }
                else
                {
                    CreateUIToggles(attributeAndMethod);
                }
            }
        }

        private void CreateUIButtons(KeyValuePair<AnimationUtilityButtonAttribute, MethodInfo> attributeAndMethod)
        {
            GameObject buttonGameObject = Instantiate(buttonPrefab);
            buttonGameObject.name = attributeAndMethod.Value.Name;

            Text text = buttonGameObject.GetComponentInChildren<Text>();
            text.text = attributeAndMethod.Value.Name;

            AnimatorControllerButton controllerButton = buttonGameObject.GetComponent<AnimatorControllerButton>();
            controllerButton.OnButtonClicked += ButtonClicked;

            buttonGameObject.transform.SetParent(animatorControlButtonsParent, false);
            buttons.Add(buttonGameObject);
            buttonGameObject.SetActive(false);
        }

        private void CreateUIToggles(KeyValuePair<AnimationUtilityButtonAttribute, MethodInfo> attributeAndMethod)
        {
            GameObject toggleGameObject = Instantiate(togglePrefab);
            toggleGameObject.name = attributeAndMethod.Value.Name;

            Text text = toggleGameObject.GetComponentInChildren<Text>();
            text.text = attributeAndMethod.Value.Name;

            AnimatorControllerToggle controllerToggle = toggleGameObject.GetComponent<AnimatorControllerToggle>();
            controllerToggle.OnToggleClicked += ToggleClicked;

            toggleGameObject.transform.SetParent(animatorControlTogglesParent, false);
            toggles.Add(toggleGameObject);
            toggleGameObject.SetActive(false);
        }

        private void OnEnable()
        {
            AddEventHandlers();
            SetControlsActive(false);
        }

        private void AddEventHandlers()
        {
            shootArea.OnShootAreaClick += ShootAreaOnShootAreaClicked;
        }

        private void ShootAreaOnShootAreaClicked()
        {
            OnShootAreaClicked?.Invoke();
        }

        private void ButtonClicked(string animationName)
        {
            OnAnimationControllerButtonClicked?.Invoke(animationName);
        }

        private void ToggleClicked(string animationName, bool isActive)
        {
            OnAnimationControllerToggleClicked(animationName, isActive);
        }
    }
}
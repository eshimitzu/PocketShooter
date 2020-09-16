using System;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class AnimatorControllerToggle : MonoBehaviour
    {
        public Action<string, bool> OnToggleClicked;

        private string toggleName;

        private void Start()
        {
            Toggle toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnValueChanged);

            toggleName = gameObject.name;
        }

        private void OnValueChanged(bool isActive)
        {
            OnToggleClicked?.Invoke(toggleName, isActive);
        }
    }
}

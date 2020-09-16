using System;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class AnimatorControllerButton : MonoBehaviour
    {
        public Action<string> OnButtonClicked;

        private string buttonName;

        private void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);

            buttonName = gameObject.name;
        }

        private void OnClick()
        {
            OnButtonClicked?.Invoke(buttonName);
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    public class SpeedControlsView : MonoBehaviour
    {
        public Action<float> OnCurrentMoveSpeedChanged;
        public Action<bool> OnRunToggleChanged;

        [SerializeField]
        private Slider moveSpeedSlider;
        [SerializeField]
        private Text currentMoveSpeedText;
        [SerializeField]
        private Toggle runToggle;

        private bool canChangeSlider = true;

        public void SetControlsActive(bool activeSelf)
        {
            moveSpeedSlider.gameObject.SetActive(activeSelf);
            currentMoveSpeedText.gameObject.SetActive(activeSelf);
            runToggle.gameObject.SetActive(activeSelf);
        }

        public void ResetSpeedControls(float maxSliderValue, float midSliderValue)
        {
            moveSpeedSlider.maxValue = maxSliderValue;
            moveSpeedSlider.value = midSliderValue;
        }

        private void OnEnable()
        {
            AddEventHandlers();
            SetControlsActive(false);
        }

        private void AddEventHandlers()
        {
            moveSpeedSlider.onValueChanged.AddListener(SpeedSlider_OnValueChanged);
            runToggle.onValueChanged.AddListener(RunToggle_OnValueChanged);
        }

        private void SpeedSlider_OnValueChanged(float speed)
        {
            OnCurrentMoveSpeedChanged?.Invoke(speed);
            currentMoveSpeedText.text = string.Format("{0:f2}", speed);

            if (speed < 1f && runToggle.isOn)
            {
                canChangeSlider = false;
                runToggle.isOn = false;
            }
            else if (speed >= 1f)
            {
                runToggle.isOn = true;
            }
        }

        private void RunToggle_OnValueChanged(bool isActive)
        {
            if (isActive && moveSpeedSlider.value < 1f)
            {
                canChangeSlider = true;
            }

            OnRunToggleChanged?.Invoke(isActive);

            if (canChangeSlider)
            {
                float speed = isActive ? 1f : 0.5f;
                currentMoveSpeedText.text = string.Format("{0:f2}", speed);

                moveSpeedSlider.value = speed;
            }

            canChangeSlider = true;
        }
    }
}
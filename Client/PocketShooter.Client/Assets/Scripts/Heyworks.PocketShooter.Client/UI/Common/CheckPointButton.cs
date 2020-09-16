using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Settings
{
    public class CheckPointButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        [SerializeField]
        private Image checkPoint;

        public bool IsCheckPointActive
        {
            set
            {
                checkPoint.gameObject.SetActive(value);
            }
        }

        public event Action OnClick;

        private void OnEnable()
        {
            button.onClick.AddListener(ButtonOnClick);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(ButtonOnClick);
        }

        private void ButtonOnClick()
        {
            OnClick?.Invoke();
        }
    }
}
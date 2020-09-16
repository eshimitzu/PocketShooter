using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD.Buttons
{
    public class UIButton : Button
    {
        [SerializeField]
        private UIButton.ButtonDownEvent m_OnDown = new UIButton.ButtonDownEvent();

        [SerializeField]
        private UIButton.ButtonUpEvent m_OnUp = new UIButton.ButtonUpEvent();

        public bool IsPointerDown { get; private set; }

        public UIButton.ButtonDownEvent onDown
        {
            get { return this.m_OnDown; }
            set { this.m_OnDown = value; }
        }

        public UIButton.ButtonUpEvent onUp
        {
            get { return this.m_OnUp; }
            set { this.m_OnUp = value; }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            IsPointerDown = true;

            if (IsInteractable())
            {
                m_OnDown.Invoke();
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            IsPointerDown = false;

            if (IsInteractable())
            {
                m_OnUp.Invoke();
            }
        }

        protected override void InstantClearState()
        {
            base.InstantClearState();
            IsPointerDown = false;
        }

        [Serializable]
        public class ButtonDownEvent : UnityEvent
        {
        }

        [Serializable]
        public class ButtonMoveEvent : UnityEvent
        {
        }

        [Serializable]
        public class ButtonUpEvent : UnityEvent
        {
        }
    }
}
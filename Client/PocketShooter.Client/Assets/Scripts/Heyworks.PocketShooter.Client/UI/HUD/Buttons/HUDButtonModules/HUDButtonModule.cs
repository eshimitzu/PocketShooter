using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD.Buttons.HUDButtonModules
{
    internal class HUDButtonModule : MonoBehaviour
    {
        [SerializeField]
        private UIButton button;

        [SerializeField]
        private Image skillIcon;

        [SerializeField]
        private Image back;

        [SerializeField]
        private float skillIconNotActiveColorAlpha = 0.5f;
        
        public Image SkillIcon => skillIcon;

        public event Action ButtonClick;

        public event Action ButtonDown;

        public event Action ButtonUp;

        public bool IsInteractable { get; private set; }

        private void OnEnable()
        {
            button.onClick.AddListener(ButtonOnClick);
            button.onDown.AddListener(ButtonOnDown);
            button.onUp.AddListener(ButtonOnUp);
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        protected virtual void OnDisable()
        {
            button.onClick.RemoveListener(ButtonOnClick);
            button.onDown.RemoveListener(ButtonOnDown);
            button.onUp.RemoveListener(ButtonOnUp);

            IsInteractable = true;
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void MakeSemiTransparent(bool isSemiTransparent)
        {
            Color old = SkillIcon.color;
            SkillIcon.color = new Color(old.r, old.g, old.b, isSemiTransparent ? skillIconNotActiveColorAlpha : 1f);
        }

        public void MakeBackSemiTransparent(bool isSemiTransparent)
        {
            Color old = SkillIcon.color;
            back.color = new Color(old.r, old.g, old.b, isSemiTransparent ? skillIconNotActiveColorAlpha : 1f);
        }

        public void SetIsInterectable(bool isInteractable, bool isBackSemiTransparent = false)
        {
            IsInteractable = isInteractable;

            MakeSemiTransparent(!isInteractable);

            if (isBackSemiTransparent)
            {
                MakeBackSemiTransparent(!isInteractable);
            }

            button.interactable = isInteractable;
        }

        private void ButtonOnClick()
        {
            ButtonClick?.Invoke();
        }

        private void ButtonOnDown()
        {
            ButtonDown?.Invoke();
        }

        private void ButtonOnUp()
        {
            ButtonUp?.Invoke();
        }
    }
}
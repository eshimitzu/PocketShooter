using System;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.UI.HUD.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD
{
    /// <summary>
    /// Represents view containing different HUD controls in battle.
    /// </summary>
    internal class BattleHUDView : MonoBehaviour, IScopeControl
    {
        [SerializeField]
        private TrooperInfoBarView trooperInfoBarView;
        [SerializeField]
        private Text nicknamelabel;
        [SerializeField]
        private HUDButtonCooldown skillButtonFirst;
        [SerializeField]
        private HUDButtonCooldown skillButtonSecond;
        [SerializeField]
        private HUDButtonCooldown skillButtonThird;
        [SerializeField]
        private HUDButtonCooldown skillButtonFourth;
        [SerializeField]
        private HUDButtonCooldown skillButtonFifth;
        [SerializeField]
        private UIButton scopeButton;
        [SerializeField]
        private CrosshairView crosshairView;
        [SerializeField]
        private Button menuButton;
        [SerializeField]
        private KillsView killsView;
        [SerializeField]
        private BattleNotificationView battleNotificationView;

        /// <summary>
        /// Gets the skill button first.
        /// </summary>
        /// <value>The skill button first.</value>
        public HUDButtonCooldown SkillButtonFirst => skillButtonFirst;

        /// <summary>
        /// Gets the skill button second.
        /// </summary>
        /// <value>The skill button second.</value>
        public HUDButtonCooldown SkillButtonSecond => skillButtonSecond;

        /// <summary>
        /// Gets the skill button third.
        /// </summary>
        /// <value>The skill button third.</value>
        public HUDButtonCooldown SkillButtonThird => skillButtonThird;

        public HUDButtonCooldown SkillButtonFourth => skillButtonFourth;

        public HUDButtonCooldown SkillButtonFifth => skillButtonFifth;

        /// <summary>
        /// Gets the TrooperInfo bar view.
        /// </summary>
        /// <value>The ammo bar view.</value>
        public TrooperInfoBarView TrooperInfoBarView => trooperInfoBarView;

        public CrosshairView CrosshairView => crosshairView;

        public BattleNotificationView BattleNotificationView => battleNotificationView;

        /// <summary>
        /// ReloadWeapon action.
        /// </summary>
        public event Action ReloadWeapon;

        /// <summary>
        /// OpenMenu action.
        /// </summary>
        public event Action OpenMenu;

        public event Action Scope;

        private void OnEnable()
        {
            trooperInfoBarView.ReloadButtonClick += TrooperInfoBarView_ReloadButtonClick;
            menuButton.onClick.AddListener(OnMenuButtonClick);
            scopeButton.onClick.AddListener(ScopeButtonOnClick);
        }

        private void OnDisable()
        {
            trooperInfoBarView.ReloadButtonClick -= TrooperInfoBarView_ReloadButtonClick;
            menuButton.onClick.RemoveListener(OnMenuButtonClick);
            scopeButton.onClick.RemoveListener(ScopeButtonOnClick);
        }

        /// <summary>
        /// UpdateHealth.
        /// </summary>
        /// <param name="health">health.</param>
        /// <param name="armor">armor.</param>
        /// <param name="maxHealth">Max player health.</param>
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void SetHealthViewBars(float maxHealth, float maxArmor, int healthForSegment, int armorForSegment)
        {
            trooperInfoBarView.SetHealthViewBars(maxHealth, maxArmor, healthForSegment, armorForSegment);
        }

        public void UpdateHealth(float health, float armor, float maxHealth, float maxArmor)
        {
            trooperInfoBarView.UpdateHealth(health, armor, maxHealth, maxArmor);
        }

        /// <summary>
        /// UpdateAmmo.
        /// </summary>
        /// <param name="ammoInClip">ammoInClip.</param>
        /// <param name="clipSize">clipSize.</param>
        public void UpdateAmmo(int ammoInClip, int clipSize)
        {
            trooperInfoBarView.UpdateAmmo(ammoInClip, clipSize);
        }

        /// <summary>
        /// SetNickname.
        /// </summary>
        /// <param name="nickname">nickname.</param>
        public void SetNickname(string nickname)
        {
            nicknamelabel.text = nickname;
        }

        /// <summary>
        /// Update crosshair progress.
        /// </summary>
        /// <param name="progress">Progress value.</param>
        public void UpdateCrosshairProgress(float progress)
        {
            crosshairView.WarmupProgress = progress;
        }

        public void SetCrosshairImage(Sprite crosshair, float size)
        {
            crosshairView.SetCrosshair(crosshair, size);
        }

        public void ShowKill(string killer, string victim, bool isKillerTeamMy, bool isVictimTeamMy, bool isLocalPlayerKiller, bool isLocalPlayerVictim)
        {
            killsView.AddKill(killer, victim, isKillerTeamMy, isVictimTeamMy, isLocalPlayerKiller, isLocalPlayerVictim);
        }

        public void SetScopeButtonVisible(bool isVisible)
        {
            scopeButton.gameObject.SetActive(isVisible);
        }

        private void TrooperInfoBarView_ReloadButtonClick()
        {
            ReloadWeapon?.Invoke();
        }

        private void OnMenuButtonClick()
        {
            OpenMenu?.Invoke();
        }

        private void ScopeButtonOnClick()
        {
            Scope?.Invoke();
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadWeapon?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                InvokeButton(skillButtonFirst.ButtonModule.GetComponent<UIButton>());
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                InvokeButton(skillButtonSecond.ButtonModule.GetComponent<UIButton>());
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                InvokeButton(skillButtonThird.ButtonModule.GetComponent<UIButton>());
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                InvokeButton(skillButtonFourth.ButtonModule.GetComponent<UIButton>());
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                InvokeButton(skillButtonFifth.ButtonModule.GetComponent<UIButton>());
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                InvokeButton(scopeButton);
            }
        }

        private void InvokeButton(UIButton button)
        {
            if (button.interactable)
            {
                button.onDown.Invoke();
                button.onUp.Invoke();
                button.onClick.Invoke();
            }
        }
#endif
    }
}

using System;
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD
{
    public class TrooperInfoBarView : MonoBehaviour
    {
        [SerializeField]
        private Text ammoLabel;
        [SerializeField]
        private Button reloadButton;
        [SerializeField]
        private GameObject reloadIcon;
        [SerializeField]
        private HealthBar healthProgressBar;
        [SerializeField]
        private HealthBar armorProgressBar;
        [SerializeField]
        private RectTransform healthRectTransform;
        [SerializeField]
        private RectTransform armorRectTransform;
        [SerializeField]
        private GameObject eyeLashPrefab;
        [SerializeField]
        private Vector2 eyeLashSize;

        public event Action ReloadButtonClick;

        private void OnEnable()
        {
            reloadButton.onClick.AddListener(OnReloadButtonClick);
        }

        private void OnDisable()
        {
            reloadButton.onClick.RemoveListener(OnReloadButtonClick);
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void UpdateAmmo(int magazineAmmoValue, int maxMagazineAmmoValue)
        {
            ammoLabel.text = magazineAmmoValue + "<size=50>/" + maxMagazineAmmoValue + "</size> ";
        }

        public void SetReloadState(bool isActive)
        {
            reloadIcon.SetActive(isActive);
            reloadButton.enabled = isActive;

            if (!isActive)
            { 
                ammoLabel.text = "<size=90>∞</size> ";
            }
        }

        public void UpdateHealth(float health, float armor, float maxHealth, float maxArmor)
        {
            healthProgressBar.UpdateProgress(health, maxHealth);
            armorProgressBar.UpdateProgress(armor, maxArmor);
        }

        public void SetHealthViewBars(float maxHealth, float maxArmor, int healthForSegment, int armorForSegment)
        {
            healthProgressBar.SetEyeLashesForHP(maxHealth, healthForSegment, eyeLashSize.x, eyeLashSize.y);
            armorProgressBar.SetEyeLashesForHP(maxArmor, armorForSegment, eyeLashSize.x, eyeLashSize.y);
        }

        private void OnReloadButtonClick()
        {
            ReloadButtonClick?.Invoke();
        }
    }
}
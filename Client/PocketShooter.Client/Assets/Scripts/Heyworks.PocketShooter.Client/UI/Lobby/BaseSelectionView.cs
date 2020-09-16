using Heyworks.PocketShooter.Skills;
using UnityEngine;
using UnityEngine.UI;
using Heyworks.PocketShooter.UI.Components;

namespace Heyworks.PocketShooter.UI
{
    public class BaseSelectionView : MonoBehaviour
    {
        [SerializeField]
        private Text itemName;

        [SerializeField]
        private LobbyAmmunitionButton weaponButton;

        [SerializeField]
        private LobbyAmmunitionButton helmetButton;

        [SerializeField]
        private LobbyAmmunitionButton armorButton;

        [SerializeField]
        private LobbyMainCard mainCard;

        [SerializeField]
        private LobbySkillButton skillButton1;

        [SerializeField]
        private LobbySkillButton skillButton2;

        [SerializeField]
        private LobbySkillButton skillButton3;

        [SerializeField]
        private RectTransform itemPreviewRoot;

        [SerializeField]
        private RectTransform itemsBar;

        [SerializeField]
        private RectTransform skillsBar;

        [SerializeField]
        private SkillControllerFactory skillFactory;

        [SerializeField]
        private Image itemPreview;

        // TODO: v.filipooiv Transfer to popup system
        [SerializeField]
        private ItemTooltipView itemTooltipView;

        public Text ItemName => itemName;

        public LobbyAmmunitionButton WeaponButton => weaponButton;

        public LobbyAmmunitionButton HelmetButton => helmetButton;

        public LobbyAmmunitionButton ArmorButton => armorButton;

        public LobbyMainCard MainCard => mainCard;

        public LobbySkillButton SkillButton1 => skillButton1;

        public LobbySkillButton SkillButton2 => skillButton2;

        public LobbySkillButton SkillButton3 => skillButton3;

        public RectTransform ItemPreviewRoot => itemPreviewRoot;

        public RectTransform ItemsBar => itemsBar;

        public RectTransform SkillsBar => skillsBar;

        public SkillControllerFactory SkillFactory => skillFactory;

        public Image ItemPreview => itemPreview;

        public ItemTooltipView ItemTooltipView => itemTooltipView;
    }
}
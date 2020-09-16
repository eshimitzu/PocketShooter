using Heyworks.PocketShooter.UI.Common;
using Heyworks.PocketShooter.UI.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyButtonsView : MonoBehaviour
    {
        [SerializeField]
        private LobbyColorsPreset colorsPreset;

        [SerializeField]
        private ShopPurchaseButton purchaseButton;

        [SerializeField]
        private Button equipButton;

        [SerializeField]
        private Text equipButtonLabel;

        [SerializeField]
        private AdvancedImage equipButtonBackground;

        [SerializeField]
        private AdvancedShadow equipButtonShadow;

        [SerializeField]
        private ShopPurchaseButton levelUpInstantButton;

        [SerializeField]
        private ShopPurchaseButton levelUpRegularButton;

        [SerializeField]
        private ShopPurchaseButton gradeUpInstantButton;

        [SerializeField]
        private TimerView progressTimerView;

        [SerializeField]
        private AutoRefreshPurchaseButtonController completeProgressButtonController;

        public LobbyColorsPreset ColorsPreset => colorsPreset;

        public ShopPurchaseButton PurchaseButton => purchaseButton;

        public Button EquipButton => equipButton;

        public Text EquipButtonLabel => equipButtonLabel;

        public AdvancedImage EquipButtonBackground => equipButtonBackground;

        public AdvancedShadow EquipButtonShadow => equipButtonShadow;

        public ShopPurchaseButton LevelUpInstantButton => levelUpInstantButton;

        public ShopPurchaseButton LevelUpRegularButton => levelUpRegularButton;

        public ShopPurchaseButton GradeUpInstantButton => gradeUpInstantButton;

        public TimerView ProgressTimerView => progressTimerView;

        public AutoRefreshPurchaseButtonController CompleteProgressButtonController => completeProgressButtonController;
    }
}
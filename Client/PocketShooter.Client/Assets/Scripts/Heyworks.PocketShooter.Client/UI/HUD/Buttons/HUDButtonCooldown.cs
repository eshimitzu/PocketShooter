using Heyworks.PocketShooter.UI.HUD.Buttons.HUDButtonModules;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.HUD.Buttons
{
    internal class HUDButtonCooldown : MonoBehaviour, IHUDButton
    {
        [SerializeField]
        private HUDButtonModule buttonModule;

        [SerializeField]
        private HUDButtonClickCounterModule clickCounterModule;

        [SerializeField]
        private HUDButtonProgressModule progressModule;

        [SerializeField]
        private HUDButtonCooldownModule cooldownModule;

        [SerializeField]
        private HUDButtonBlinkModule blinkModule;

        [SerializeField]
        private HUDButtonLabelModule labelModule;

        public HUDButtonModule ButtonModule => buttonModule;

        public HUDButtonProgressModule ProgressModule => progressModule;

        public HUDButtonCooldownModule CooldownModule => cooldownModule;

        public HUDButtonClickCounterModule ClickCounterModule => clickCounterModule;

        public HUDButtonBlinkModule BlinkModule => blinkModule;

        public HUDButtonLabelModule LabelModule => labelModule;
    }
}
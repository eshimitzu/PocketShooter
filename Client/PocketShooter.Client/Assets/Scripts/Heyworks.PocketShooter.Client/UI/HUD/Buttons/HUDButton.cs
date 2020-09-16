using Heyworks.PocketShooter.UI.HUD.Buttons.HUDButtonModules;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.HUD.Buttons
{
    internal class HUDButton : MonoBehaviour, IHUDButton
    {
        [SerializeField]
        private HUDButtonModule buttonModule;

        public HUDButtonModule ButtonModule => buttonModule;
    }
}
using Heyworks.PocketShooter.Utils;
using UnityEngine;

namespace Heyworks.PocketShooter.UI.HUD.Buttons.HUDButtonModules
{
    internal class HUDButtonBlinkModule : MonoBehaviour
    {
        [SerializeField]
        private ColorUpdateEffect colorUpdateEffect;

        public bool IsEffectActive { get; private set; }

        public void StartBlink()
        {
            IsEffectActive = true;

            colorUpdateEffect.Play();
        }

        public void StopBlink()
        {
            IsEffectActive = false;

            colorUpdateEffect.Stop();
        }
    }
}
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Core.SchedulerManager;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD.Buttons.HUDButtonModules
{
    internal class HUDButtonLabelModule : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        public void SetLabel(string text)
        {
            label.text = text;
        }
    }
}
using System.Diagnostics.CodeAnalysis;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD.Buttons.HUDButtonModules
{
    internal class HUDButtonClickCounterModule : MonoBehaviour
    {
        [SerializeField]
        private Text clickCounterLabel;

        private StringNumbersCache numberCache;

        private void Awake()
        {
            numberCache = new StringNumbersCache();
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void SetRemainingClicks(int remainingClicks, int maxClicks)
        {
            clickCounterLabel.text = numberCache.GetString(remainingClicks) + "/" + numberCache.GetString(maxClicks);
        }
    }
}
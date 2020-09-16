using UnityEngine;

namespace Heyworks.PocketShooter.UI.Lobby
{
    public class SideItemProgressView : MonoBehaviour
    {
        [SerializeField]
        private TimerView progressTimerView;

        [SerializeField]
        private AutoRefreshPurchaseButtonController completeProgressButtonController;

        public TimerView ProgressTimerView => progressTimerView;

        public AutoRefreshPurchaseButtonController CompleteProgressButtonController => completeProgressButtonController;
    }
}

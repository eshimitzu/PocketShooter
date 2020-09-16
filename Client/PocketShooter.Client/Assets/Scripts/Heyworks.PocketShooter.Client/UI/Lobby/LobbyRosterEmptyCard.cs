using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyRosterEmptyCard : MonoBehaviour
    {
        [SerializeField]
        private Image placeholder;

        public Image Placeholder => placeholder;
    }
}
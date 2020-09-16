using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class LobbyOfferItem : MonoBehaviour
    {
        [SerializeField]
        private Image icon;

        [SerializeField]
        private Text label;

        [SerializeField]
        private Button button;

        public Image Icon => icon;

        public Text Label => label;

        public Button Button => button;

        public event System.Action OnButtonClick;

        private void OnEnable()
        {
            button.onClick.AddListener(ClickButton);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(ClickButton);
        }

        private void ClickButton()
        {
            OnButtonClick?.Invoke();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Components
{
    public class ResourceView : MonoBehaviour
    {
        [SerializeField]
        private Image goldIcon;
        [SerializeField]
        private Text goldAmountLabel;
        [SerializeField]
        private Image cashIcon;
        [SerializeField]
        private Text cashAmountLabel;

        public void Setup(int gold, int cash)
        {
            goldIcon.gameObject.SetActive(false);
            goldAmountLabel.gameObject.SetActive(false);
            cashIcon.gameObject.SetActive(false);
            cashAmountLabel.gameObject.SetActive(false);

            if (gold > 0)
            {
                goldIcon.gameObject.SetActive(true);
                goldAmountLabel.gameObject.SetActive(true);

                goldAmountLabel.text = gold.ToString();
            }

            if (cash > 0)
            {
                cashIcon.gameObject.SetActive(true);
                cashAmountLabel.gameObject.SetActive(true);

                cashAmountLabel.text = cash.ToString();
            }
        }

        public void Setup(int amount)
        {
            goldIcon.gameObject.SetActive(false);
            goldAmountLabel.gameObject.SetActive(true);
            cashIcon.gameObject.SetActive(false);
            cashAmountLabel.gameObject.SetActive(false);

            goldAmountLabel.text = amount.ToString();
        }
    }
}

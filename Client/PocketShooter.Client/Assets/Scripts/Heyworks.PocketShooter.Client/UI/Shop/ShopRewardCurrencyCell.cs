using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class ShopRewardCurrencyCell : MonoBehaviour
    {
        [SerializeField]
        private Image coinIcon;

        [SerializeField]
        private Image bucksIcon;

        [SerializeField]
        private Text amountLabel;

        //public void Show(RewardType rewardType, int amount)
        //{
        //    switch (rewardType)
        //    {
        //        case RewardType.Cash:
        //            bucksIcon.gameObject.SetActive(true);
        //            coinIcon.gameObject.SetActive(false);
        //            break;
        //        case RewardType.Gold:
        //            bucksIcon.gameObject.SetActive(false);
        //            coinIcon.gameObject.SetActive(true);
        //            break;
        //    }

        //    amountLabel.text = $"{amount}";
        //}
    }
}
using Heyworks.PocketShooter.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI
{
    public class ShopRewardItemCell : MonoBehaviour
    {
        [SerializeField]
        private Text powerLabel;

        [SerializeField]
        private StarsControl stars;

        public void Show(int power, int grade)
        {
            powerLabel.text = $"{power}";
            stars.Show(grade);
        }
    }
}
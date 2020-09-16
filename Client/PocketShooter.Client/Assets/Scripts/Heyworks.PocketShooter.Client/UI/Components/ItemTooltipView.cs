using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Components
{
    public class ItemTooltipView : MonoBehaviour
    {
        [SerializeField] Text titleLabel;

        [SerializeField] Text descriptionLabel;

        public Text TitleLabel => titleLabel;

        public Text DescriptionLabel => descriptionLabel;
    }
}
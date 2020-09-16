using Heyworks.PocketShooter.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Lobby
{
    public class UpgradeContentCard : MonoBehaviour
    {
        [SerializeField]
        private Text cardNameLabel;

        [SerializeField]
        private Image itemIcon;

        [SerializeField]
        private LevelUpView levelUpView;

        [SerializeField]
        private GradeUpView gradeUpView;

        public Text CardNameLabel => cardNameLabel;

        public LevelUpView LevelUpView => levelUpView;

        public GradeUpView GradeUpView => gradeUpView;

        public Image ItemIcon => itemIcon;
    }
}

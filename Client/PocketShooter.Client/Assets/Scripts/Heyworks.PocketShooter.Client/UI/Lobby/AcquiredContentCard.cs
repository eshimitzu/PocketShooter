using Heyworks.PocketShooter.UI.Components;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Lobby
{
    public class AcquiredContentCard : MonoBehaviour
    {
        [SerializeField]
        private Text cardNameLabel;

        [SerializeField]
        private Image itemIcon;

        [SerializeField]
        private LevelAndGradeView levelAndGradeView;

        [SerializeField]
        private ResourceView resourceView;

        [SerializeField]
        private GameObject resourceRoot;

        [SerializeField]
        private GameObject itemRoot;

        public Text CardNameLabel => cardNameLabel;

        public LevelAndGradeView LevelAndGradeView => levelAndGradeView;

        public ResourceView ResourceView => resourceView;

        public Image ItemIcon => itemIcon;

        public GameObject ResourceRoot => resourceRoot;

        public GameObject ItemRoot => itemRoot;
    }
}

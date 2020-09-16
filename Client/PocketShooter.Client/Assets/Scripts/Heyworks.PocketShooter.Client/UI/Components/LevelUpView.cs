using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Components
{
    public class LevelUpView : MonoBehaviour
    {
        [SerializeField]
        private Text levelTextFrom;
        [SerializeField]
        private Text levelTextTo;

        public void Setup(int levelFrom, int levelTo)
        {
            levelTextFrom.text = levelFrom.ToString();
            levelTextTo.text = levelTo.ToString();
        }
    }
}

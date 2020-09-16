using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Components
{
    public class LevelAndGradeView : MonoBehaviour
    {
        [SerializeField]
        private StarsControl stars;
        [SerializeField]
        private Text levelText;

        public void Setup(int level, int grade)
        {
            levelText.text = level.ToString();
            stars.Show(grade);
        }
    }
}

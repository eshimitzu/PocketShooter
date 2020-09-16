using UnityEngine;

namespace Heyworks.PocketShooter.UI.Components
{
    public class GradeUpView : MonoBehaviour
    {
        [SerializeField]
        private StarsControl starsFrom;
        [SerializeField]
        private StarsControl starsTo;

        public void Setup(Grade gradeFrom, Grade gradeTo)
        {
            starsFrom.Show((int)gradeFrom);
            starsTo.Show((int)gradeTo);
        }
    }
}

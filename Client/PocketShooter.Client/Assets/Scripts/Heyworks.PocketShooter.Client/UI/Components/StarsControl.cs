using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Components
{
    public class StarsControl : MonoBehaviour
    {
        [SerializeField]
        private List<Image> stars;

        [SerializeField]
        private LobbyColorsPreset colors;

        public void Show(int grade)
        {
            for (var i = 0; i < stars.Count; i++)
            {
                Image star = stars[i];
                star.color = colors.GetStarsColor(grade >= i);
            }
        }
    }
}
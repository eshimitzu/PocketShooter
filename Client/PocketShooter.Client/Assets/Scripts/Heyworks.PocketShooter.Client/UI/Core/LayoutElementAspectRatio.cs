using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Core
{
    public class LayoutElementAspectRatio : MonoBehaviour
    {
        [SerializeField]
        private AspectMode aspectMode;

        [SerializeField]
        private float aspectRatio;

        [SerializeField]
        private LayoutElement layoutElement;

        [SerializeField]
        private RectTransform rectTransform;

        private void Start()
        {
            StartCoroutine(SetSize());
        }

        private IEnumerator SetSize()
        {
            yield return null;

            if (aspectMode == AspectMode.HeightControlWidth)
            {
                layoutElement.preferredWidth = rectTransform.rect.size.y * aspectRatio;
            }
            else
            {
                layoutElement.preferredHeight = rectTransform.rect.size.x * aspectRatio;
            }
        }

        private enum AspectMode
        {
            HeightControlWidth,
            WidthControlHeight,
        }
    }
}
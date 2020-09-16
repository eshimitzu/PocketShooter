using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Core
{
    public class UICameraScale : MonoBehaviour
    {
        private void Awake()
        {
            var canvas = GetComponent<Canvas>();
            var canvasScaler = GetComponent<CanvasScaler>();

            // cause we fit width
            float aspect = canvasScaler.referenceResolution.x / Screen.width;
            canvas.worldCamera.orthographicSize = Screen.height / 2f * aspect;
        }
    }
}
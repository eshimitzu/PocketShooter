using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    [ExecuteInEditMode]
    public class DepthCamera : MonoBehaviour
    {
        [SerializeField]
        private DepthTextureMode mode;

        private void Start()
        {
            var cam = GetComponent<UnityEngine.Camera>();
            cam.depthTextureMode = mode;
        }
    }
}
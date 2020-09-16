using Heyworks.PocketShooter.Realtime.Data;
using UnityEngine;

namespace Heyworks.PocketShooter.Audio
{
    /// <summary>
    /// Represents manager for playing footsteps sfx.
    /// </summary>
    public class FootstepsAudioManager : MonoBehaviour
    {
        [SerializeField]
        private TrooperClass trooperClass = TrooperClass.Rambo;

        private Animator animator;
        private float lastFrameCurvSign;
        private int curveId;

        public void SetTrooperClass(TrooperClass trooperClass)
        {
            this.trooperClass = trooperClass;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            curveId = Animator.StringToHash("PlayFootsteps");
        }

        private void Update()
        {
            var currentFrameCurvSign = animator.GetFloat(curveId);

            if (Mathf.Sign(lastFrameCurvSign) != Mathf.Sign(currentFrameCurvSign))
            {
                AudioController.Instance.PostEvent(AudioKeys.Event.PlayFootsteps + trooperClass, gameObject);
            }

            lastFrameCurvSign = currentFrameCurvSign;
        }
    }
}
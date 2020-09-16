using Heyworks.PocketShooter.Audio;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    public class CharacterCommon : MonoBehaviour
    {
        [SerializeField]
        private AnimationController animationController;
        [SerializeField]
        private Transform cameraFollowPoint;
        [SerializeField]
        private Transform shotOriginTransfrom;
        [SerializeField]
        private Transform headHintTransform;
        [SerializeField]
        private Transform bodyHintTransform;
        [SerializeField]
        private CharacterAudioController audioController;

        public CharacterAudioController AudioController => audioController;

        public AnimationController AnimationController => animationController;

        public Transform CameraFollowPoint => cameraFollowPoint;

        public Transform ShotOriginTransfrom => shotOriginTransfrom;

        public Transform HeadHintTransform => headHintTransform;

        public Transform BodyHintTransform => bodyHintTransform;

        public TrooperAvatar TrooperAvatar { get; private set; }

        public void Setup(TrooperAvatar trooperAvatar)
        {
            TrooperAvatar = trooperAvatar;
            TrooperAvatar.transform.parent = transform;
        }
    }
}
using Heyworks.PocketShooter.Character;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Utilities.AnimationUtility
{
    [RequireComponent(typeof(PocketCharacterController))]
    public class OfflineLocalCharacter : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<Object, OfflineLocalCharacter>
        {
        }

        [SerializeField]
        private PocketCharacterController characterController;

        [SerializeField]
        private CharacterCommon characterCommon;

        public PocketCharacterController CharacterController => characterController;

        public CharacterCommon CharacterCommon => characterCommon;

        public void ForcePosition(Transform targetTransform)
        {
            transform.position = targetTransform.position;
            characterController.SetPosition(targetTransform.position);
        }
    }
}
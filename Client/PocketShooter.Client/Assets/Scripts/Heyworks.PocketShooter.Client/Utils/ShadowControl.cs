using Heyworks.PocketShooter.Character;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    public class ShadowControl : MonoBehaviour
    {
        [SerializeField]
        private Transform heightRoot;
 
        [SerializeField]
        private Transform bodyShadow;

        [SerializeField]
        private Transform leftLegShadow;

        [SerializeField]
        private Transform rightLegShadow;

        [SerializeField]
        private Transform leftArmShadow;

        [SerializeField]
        private Transform rightArmShadow;

        private CharacterCommon characterCommon;
        private Animator animator;

        private Transform chest;
        private Transform leftLeg;
        private Transform rightLeg;
        private Transform leftArm;
        private Transform rightArm;

        private void Start()
        {
            characterCommon = GetComponentInParent<CharacterCommon>();
            animator = characterCommon.TrooperAvatar.Animator;

            chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            leftLeg = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            rightLeg = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            leftArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            rightArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
        }

        private void LateUpdate()
        {
            float h = heightRoot.position.y;

            bodyShadow.position = new Vector3(chest.position.x, h, chest.position.z);
            leftLegShadow.position = new Vector3(leftLeg.position.x, h, leftLeg.position.z);
            rightLegShadow.position = new Vector3(rightLeg.position.x, h, rightLeg.position.z);

            leftArmShadow.position = new Vector3(leftArm.position.x, h, leftArm.position.z);
            rightArmShadow.position = new Vector3(rightArm.position.x, h, rightArm.position.z);

            var lp = rightArmShadow.localPosition;
            rightArmShadow.localPosition = new Vector3(lp.x * 0.7f, lp.y, lp.z * 0.7f);
        }
    }
}
using Heyworks.PocketShooter.Utils.Unity.EditorTools.Attributes;
using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    [CreateAssetMenu(fileName = "SkillSpecInvisibility", menuName = "Heyworks/Skills/Skill Spec Invisibility")]
    public class InvisibilitySkillSpec : SkillSpec
    {
        [SerializeField]
        private Material invisibilityMaterial;

        [SerializeField]
        [Layer]
        private int aimAssistLayer;

        [SerializeField]
        [Layer]
        private int invisibleLayer;

        public Material InvisibilityMaterial => invisibilityMaterial;

        public int AimAssistLayer => aimAssistLayer;

        public int InvisibleLayer => invisibleLayer;
    }
}

using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    // TODO: a.dezhurko separate into multiple parts for UI, Visualization, Behaviour
    [CreateAssetMenu(fileName = "SkillSpec", menuName = "Heyworks/Skills/Skill Spec")]
    public class SkillSpec : ScriptableObject
    {
        [SerializeField]
        private SkillName skillName;

        [SerializeField]
        private Sprite icon;

        public SkillName SkillName => skillName;

        public Sprite Icon => icon;
    }
}

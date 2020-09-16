using UnityEngine;

namespace Heyworks.PocketShooter.Modules.DamageHints
{
    [CreateAssetMenu(fileName = "DamageHintSettings", menuName = "Heyworks/Settings/Damage Hint Settings")]
    internal class DamageHintSettings : ScriptableObject
    {
        [SerializeField]
        private Vector3 initScale;

        [SerializeField]
        private AnimationCurve fadeCurve;

        [SerializeField]
        private AnimationCurve scaleCurve;

        [SerializeField]
        private float upScaleMultiplier;

        [SerializeField]
        private float showingTime;

        [SerializeField]
        private float defaultDelayBetweenHints;

        [SerializeField]
        private float spawnDispersionRadius;

        [SerializeField]
        private Color textColor;

        [SerializeField]
        private Color textCriticalColor;

        [SerializeField]
        private Color textExtraColor;

        [SerializeField]
        private Color textHealColor;

        [SerializeField]
        private float extraHintScaleFactor;

        [SerializeField]
        private float damageOffsetDistance;

        [SerializeField]
        private float healOffsetDistance;

        public Vector3 InitScale => initScale;

        public AnimationCurve FadeCurve => fadeCurve;

        public AnimationCurve ScaleCurve => scaleCurve;

        public float UpScaleMultiplier => upScaleMultiplier;

        public float DefaultDelayBetweenHints => defaultDelayBetweenHints;

        public float ShowingTime => showingTime;

        public Color TextColor => textColor;

        public Color TextExtraColor => textExtraColor;

        public Color TextHealColor => textHealColor;

        public Color TextCriticalColor => textCriticalColor;

        public float SpawnDispersionRadius => spawnDispersionRadius;

        public float DamageOffsetDistance => damageOffsetDistance;

        public float HealOffsetDistance => healOffsetDistance;

        public float ExtraHintScaleFactor => extraHintScaleFactor;
    }
}

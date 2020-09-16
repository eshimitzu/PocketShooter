using UnityEngine;

namespace Heyworks.PocketShooter.Configuration
{
    [System.Serializable]
    public class TrooperMaterialConfig
    {
        [SerializeField]
        private Shader highlightedShader;
        [SerializeField]
        private Color enemyHighlightRimColor;
        [SerializeField]
        private Color enemyHighlightOutlineColor;
        [SerializeField]
        private Color friendHighlightRimColor;
        [SerializeField]
        private Color friendHighlightOutlineColor;
        [SerializeField]
        private float rimPower;
        [SerializeField]
        private float outlineWidth;

        public Shader HighlightedShader => highlightedShader;

        public Color EnemyHighlightRimColor => enemyHighlightRimColor;

        public Color EnemyHighlightOutlineColor => enemyHighlightOutlineColor;

        public Color FriendHighlightRimColor => friendHighlightRimColor;

        public Color FriendHighlightOutlineColor => friendHighlightOutlineColor;

        public float RimPower => rimPower;

        public float OutlineWidth => outlineWidth;
    }
}
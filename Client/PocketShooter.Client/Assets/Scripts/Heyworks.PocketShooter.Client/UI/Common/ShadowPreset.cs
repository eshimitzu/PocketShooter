using UnityEngine;

namespace Heyworks.PocketShooter.UI.Common
{
    [CreateAssetMenu(fileName = "ShadowPreset", menuName = "Heyworks/UI/Shadow Preset")]
    public class ShadowPreset : ScriptableObject
    {
        [SerializeField]
        private Color effectColor = new Color(0.0f, 0.0f, 0.0f, 0.5f);

        [SerializeField]
        private Vector2 effectDistance = new Vector2(1f, -1f);

        public Color EffectColor => effectColor;

        public Vector2 EffectDistance => effectDistance;
    }
}
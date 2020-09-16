using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Common
{
    public class AdvancedShadow : Shadow
    {
        [SerializeField]
        private ShadowPreset shadowPreset;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            Color color = shadowPreset ? shadowPreset.EffectColor : effectColor;
            Vector2 distance = shadowPreset ? shadowPreset.EffectDistance : effectDistance;

            // TODO: use cached lists
            List<UIVertex> uiVertexList = new List<UIVertex>();
            vh.GetUIVertexStream(uiVertexList);
            ApplyShadow(uiVertexList, color, 0, uiVertexList.Count, distance.x, distance.y);
            vh.Clear();
            vh.AddUIVertexTriangleStream(uiVertexList);
        }

        #if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            var assets = UnityEditor.AssetDatabase.FindAssets("ShadowPreset", new []{"Assets/Settings"});
            if (assets.Length > 0)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[0]);
                shadowPreset = UnityEditor.AssetDatabase.LoadAssetAtPath<ShadowPreset>(path);
            }
        }
        #endif
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.Common
{
    public class AdvancedOutline : Outline
    {
        [SerializeField]
        private int outlineSize;

        private Image image;
        private List<UIVertex> uiVertexList = new List<UIVertex>();

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }

            vh.GetUIVertexStream(uiVertexList);
            int num = uiVertexList.Count * 2;
            if (uiVertexList.Capacity < num)
            {
                uiVertexList.Capacity = num;
            }

            int size = GetCapacity(uiVertexList.Count);

            int count = uiVertexList.Count;
            this.ApplyBorderOutline(
                uiVertexList,
                this.effectColor,
                count - size,
                count);

            vh.Clear();
            vh.AddUIVertexTriangleStream(uiVertexList);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            image = GetComponent<Image>();
        }

        private void ApplyBorderOutline(
            List<UIVertex> verts,
            Color32 color,
            int start,
            int end)
        {
            int num = verts.Count + end - start;
            if (verts.Capacity < num)
            {
                verts.Capacity = num;
            }

            Vector3 summ = Vector3.zero;
            for (int index = start; index < end; ++index)
            {
                summ += verts[index].position;
            }

            Vector3 middle = summ / (end - start);

            for (int index = start; index < end; ++index)
            {
                UIVertex vert = verts[index];
                verts.Add(vert);
                Vector3 position = vert.position;

                Vector3 dir = position - middle;
                if (dir.sqrMagnitude > 0)
                {
                    position += dir.normalized * outlineSize;
                }

                vert.position = position;
                Color32 color32 = color;
                if (this.useGraphicAlpha)
                {
                    color32.a = (byte)((int)color32.a * (int)verts[index].color.a / (int)byte.MaxValue);
                }

                vert.color = color32;
                verts[index] = vert;
            }
        }

        private int GetCapacity(int defaultCapacity)
        {
            switch (image.type)
            {
                case Image.Type.Simple:
                    return 6;
                case Image.Type.Sliced:
                    return 54;
                default:
                    return defaultCapacity;
            }
        }
    }
}
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter
{
    public static class RectTransformMenu
    {
        [MenuItem("CONTEXT/RectTransform/Create Empty Child")]
        static void EmptyChild(MenuCommand command)
        {
            RectTransform transform = (RectTransform)command.context;
            GameObject g = new GameObject();
            RectTransform t = g.AddComponent<RectTransform>();
            t.SetParent(transform);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
        }


        [MenuItem("CONTEXT/RectTransform/Create Image")]
        static void ImageChild(MenuCommand command)
        {
            RectTransform transform = (RectTransform)command.context;
            GameObject g = new GameObject();
            RectTransform t = g.AddComponent<RectTransform>();
            t.SetParent(transform);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;

            g.AddComponent<Image>();
        }


        [MenuItem("CONTEXT/HorizontalOrVerticalLayoutGroup/Fixed Child")]
        static void CreateFixedElement(MenuCommand command)
        {
            HorizontalOrVerticalLayoutGroup group = (HorizontalOrVerticalLayoutGroup)command.context;
            GameObject g = new GameObject();
            RectTransform t = g.AddComponent<RectTransform>();
            t.SetParent(group.transform);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;

            LayoutElement e = g.AddComponent<LayoutElement>();

            if (group is HorizontalLayoutGroup)
            {
                e.preferredWidth = 100;
            }
            else
            {
                e.preferredHeight = 100;
            }

            g.name = "fixed";
        }

        [MenuItem("CONTEXT/HorizontalOrVerticalLayoutGroup/Flexible Child")]
        static void CreateFlexibleElement(MenuCommand command)
        {
            HorizontalOrVerticalLayoutGroup group = (HorizontalOrVerticalLayoutGroup)command.context;
            GameObject g = new GameObject();
            RectTransform t = g.AddComponent<RectTransform>();
            t.SetParent(group.transform);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;

            LayoutElement e = g.AddComponent<LayoutElement>();

            if (group is HorizontalLayoutGroup)
            {
                e.flexibleWidth = 1;
            }
            else
            {
                e.flexibleHeight = 1;
            }

            g.name = "flex";
        }
    }
}
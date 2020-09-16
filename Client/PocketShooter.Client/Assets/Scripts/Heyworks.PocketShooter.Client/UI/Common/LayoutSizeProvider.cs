using UnityEngine;
using UnityEngine.UI;

public class LayoutSizeProvider : MonoBehaviour, ILayoutElement
{
    public float minWidth { get; private set; }

    public float preferredWidth { get; private set; }

    public float flexibleWidth { get; private set; }

    public float minHeight { get; private set; }

    public float preferredHeight { get; private set; }

    public float flexibleHeight { get; private set; }

    public int layoutPriority { get; private set; }

    public void CalculateLayoutInputHorizontal()
    {
        minWidth = 0f;
        preferredWidth = 0f;
        flexibleWidth = 0f;

        ILayoutElement[] childs = gameObject.transform.GetComponentsInChildren<ILayoutElement>(false);

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                ILayoutElement childsLayoutElement = child.transform.GetComponent<ILayoutElement>();
                if (childsLayoutElement != null)
                {
                    minWidth += childsLayoutElement.minWidth;
                    preferredWidth += childsLayoutElement.preferredWidth;
                    flexibleWidth += childsLayoutElement.flexibleWidth;
                }
            }
        }
    }

    public void CalculateLayoutInputVertical()
    {
        minHeight = 0f;
        preferredHeight = 0f;
        flexibleHeight = 0f;

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                ILayoutElement childsLayoutElement = child.transform.GetComponent<ILayoutElement>();
                if (childsLayoutElement != null)
                {
                    minHeight += childsLayoutElement.minHeight;
                    preferredHeight += childsLayoutElement.preferredHeight;
                    flexibleHeight += childsLayoutElement.flexibleHeight;
                }
            }
        }
    }
}

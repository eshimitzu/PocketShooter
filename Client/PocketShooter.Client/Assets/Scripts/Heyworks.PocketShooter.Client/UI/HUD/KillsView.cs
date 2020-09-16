using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter.UI.HUD
{
    public class KillsView : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;
        [SerializeField]
        private Transform contentTransform;
        [SerializeField]
        private float contentSpacing;
        [SerializeField]
        private KillLine killLinePrefab;
        [SerializeField]
        private float killLineDisappearTime;
        [SerializeField]
        private float speed;

        private Vector3 distanceBetweenLines;

        private void Start()
        {
            distanceBetweenLines = Vector3.up * (killLinePrefab.GetComponent<RectTransform>().rect.height + contentSpacing);
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:ElementsMustBeOrderedByAccess", Justification = "Reviewed.")]
        public void AddKill(string killer, string victim, bool isKillerTeamMy, bool isVictimTeamMy, bool isLocalPlayerKiller, bool isLocalPlayerVictim)
        {
            RectTransform lastChild = null;

            if (contentTransform.childCount > 0)
            {
                lastChild = contentTransform.GetChild(contentTransform.childCount - 1).GetComponent<RectTransform>();
            }

            KillLine killLine = Instantiate(killLinePrefab, contentTransform);

            if (lastChild)
            {
                killLine.transform.localPosition = lastChild.transform.localPosition - distanceBetweenLines;
            }

            killLine.Setup(killer, victim, isKillerTeamMy, isVictimTeamMy, isLocalPlayerKiller, isLocalPlayerVictim);
        }

        private void Update()
        {
            KillLine[] childs = contentTransform.GetComponentsInChildren<KillLine>();

            for (int i = 0; i < childs.Length; i++)
            {
                float checkDistance = i == 0 ? 0f : distanceBetweenLines.y;
                float prevChildPositionY = i == 0 ? 0f : childs[i - 1].transform.localPosition.y;
                Vector3 childPosition = childs[i].transform.localPosition;

                if (Mathf.Abs(childPosition.y - prevChildPositionY) > checkDistance)
                {
                    childPosition.y = Mathf.Min(childPosition.y + speed * Time.deltaTime, prevChildPositionY - checkDistance);
                    childs[i].transform.localPosition = childPosition;
                }
            }

            if (childs.Length > 0 && !childs[0].IsDisappearing)
            {
                childs[0].StartDisappearing(killLineDisappearTime);
            }
        }
    }
}
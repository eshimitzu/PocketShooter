using Heyworks.PocketShooter;
using Heyworks.PocketShooter.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.UI;
using Heyworks.PocketShooter.UI.Localization;
using Heyworks.PocketShooter.Utils;
using UnityEngine;
using UnityEngine.UI;

public class DominationZonePointer : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;
    [SerializeField]
    private Image zoneProgressBar;
    [SerializeField]
    private Image zoneProgressBarBack;
    [SerializeField]
    private Text zoneLabel;
    [SerializeField]
    private GameVisualSettings gameVisualSettings;
    [SerializeField]
    private float heightOffset;
    [SerializeField]
    private RectTransform arrowTransform;

    private Color zoneProgressBarBackFriendColor;
    private Color zoneProgressBarBackEnemyColor;
    private Color zoneProgressBarBackDefaultColor;

    private IZone zone;
    private DominationModeInfo config;
    private TeamNo team;

    private Canvas canvase;
    private Vector3 zonePosition;
    private RectTransform parentRectTransform;
    private Rect frameRect;

    public void Setup(int index, IZone zone, DominationModeInfo config, TeamNo team)
    {
        this.zone = zone;
        this.config = config;
        this.team = team;

        string pointLocKey = null;

        switch (index)
        {
            case 0:
                pointLocKey = LocKeys.PointA;
                break;
            case 1:
                pointLocKey = LocKeys.PointB;
                break;
            case 2:
                pointLocKey = LocKeys.PointC;
                break;
        }

        if (pointLocKey != null)
        {
            zoneLabel.SetLocalizedText(pointLocKey);
        }

        Canvas[] canvases = transform.GetComponentsInParent<Canvas>();
        canvase = canvases[canvases.Length - 1];

        zonePosition = new Vector3(zone.ZoneInfo.X, zone.ZoneInfo.Y + heightOffset, zone.ZoneInfo.Z);

        frameRect = new Rect(
            rectTransform.rect.width * canvase.scaleFactor * 0.5f,
            rectTransform.rect.height * canvase.scaleFactor * 0.5f,
            Screen.width - rectTransform.rect.width * canvase.scaleFactor,
            Screen.height - rectTransform.rect.height * canvase.scaleFactor);
    }

    private void Awake()
    {
        zoneProgressBarBackFriendColor = gameVisualSettings.FriendColor;
        zoneProgressBarBackEnemyColor = gameVisualSettings.EnemyColor;
        zoneProgressBarBackDefaultColor = zoneProgressBarBack.color;
        zoneProgressBarBackEnemyColor.a = zoneProgressBarBackDefaultColor.a;
        zoneProgressBarBackFriendColor.a = zoneProgressBarBackDefaultColor.a;

        parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
    }

    private void Update()
    {
        //---------- Capture progress ----------
        float progress = zone.State.Progress / config.TimeplayersToCapture;
        zoneProgressBar.fillAmount = Mathf.Lerp(zoneProgressBar.fillAmount, progress, Time.deltaTime * 10);

        if (zone.State.OwnerTeam != TeamNo.None)
        {
            if (zone.State.OwnerTeam != team)
            {
                zoneProgressBar.color = zoneProgressBarBackEnemyColor;
            }
            else
            {
                zoneProgressBar.color = zoneProgressBarBackFriendColor;
            }
        }

        // ---------- Position ----------
        Vector3 zoneOnScreen = Camera.main.WorldToScreenPoint(zonePosition);
        Quaternion arrowRotation = Quaternion.identity;

        if (!(zoneOnScreen.z > 0 &&
            zoneOnScreen.x > frameRect.x && zoneOnScreen.x < frameRect.max.x &&
            zoneOnScreen.y > frameRect.y && zoneOnScreen.y < frameRect.max.y))
        {
            Vector2 zonePositionCameraLocal = Camera.main.transform.InverseTransformPoint(zonePosition);

            zoneOnScreen = IntersectionWithRayFromCenter(frameRect, frameRect.center + zonePositionCameraLocal);

            Vector2 pointerDirectionFromCenter = rectTransform.anchoredPosition - parentRectTransform.rect.center; 
            float arrowAngleZ = Vector2.Angle(pointerDirectionFromCenter, Vector3.right) + 90f;
            if (pointerDirectionFromCenter.y < 0f)
            {
                arrowAngleZ = -arrowAngleZ + 180f;
            }

            arrowRotation = Quaternion.Euler(0f, 0f, arrowAngleZ);
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRectTransform,
            zoneOnScreen,
            canvase.worldCamera,
            out Vector2 localPoint);

        rectTransform.anchoredPosition = localPoint;

        arrowTransform.localRotation = arrowRotation;
    }

    private Vector2 IntersectionWithRayFromCenter(Rect rect, Vector2 pointOnRay)
    {
        Vector2 pointOnRayLocal = pointOnRay - rect.center;
        Vector2 edgeToRayRatios = DividedBy(new Vector2(rect.xMax, rect.yMax) - rect.center, Abs(pointOnRayLocal));

        return (edgeToRayRatios.x < edgeToRayRatios.y) ?
            new Vector2(
                pointOnRayLocal.x > 0 ? rect.xMax : rect.xMin,
                pointOnRayLocal.y * edgeToRayRatios.x + rect.center.y) :
            new Vector2(
                pointOnRayLocal.x * edgeToRayRatios.y + rect.center.x,
                pointOnRayLocal.y > 0 ? rect.yMax : rect.yMin);
    }

    private Vector2 Abs(Vector2 vector)
    {
        for (int i = 0; i < 2; ++i)
        { 
            vector[i] = Mathf.Abs(vector[i]); 
        }

        return vector;
    }

    private Vector2 DividedBy(Vector2 vector, Vector2 divisor)
    {
        for (int i = 0; i < 2; ++i)
        {
            vector[i] /= divisor[i];
        }

        return vector;
    }
}
using Heyworks.PocketShooter.Realtime;
using Heyworks.PocketShooter.Realtime.Entities;
using Heyworks.PocketShooter.Utils;
using UnityEngine;

namespace Heyworks.PocketShooter.Modules.GameEnvironment
{
    internal class ZoneView : MonoBehaviour
    {
        public const float ScaleForOneMeter = 0.2f;
        private const string MaterialColor = "_MainColor";

        private enum ZoneColor
        {
            Neutral = 0,
            Friend = 1,
            Enemy = 2,
        }

        [SerializeField]
        private GameVisualSettings gameVisualSettings;

        private MeshRenderer meshRenderer;
        private Material material;
        private int colorPropertyId;

        private IZone zone;
        private Room room;

        private ZoneColor currentColor;

        private ZoneColor CurrentColor
        {
            get => currentColor;
            set
            {
                Color color = Color.white;

                currentColor = value;
                switch (currentColor)
                {
                    case ZoneColor.Neutral:
                        color = gameVisualSettings.NeutralColor;
                        break;
                    case ZoneColor.Friend:
                        color = gameVisualSettings.FriendCaptureZoneColor;
                        break;
                    case ZoneColor.Enemy:
                        color = gameVisualSettings.EnemyCaptureZoneColor;
                        break;
                    default:
                        break;
                }

                material.SetColor(colorPropertyId, color);
            }
        }

        public void Setup(IZone zone, Room room)
        {
            this.zone = zone;
            this.room = room;

            transform.localScale = ScaleForOneMeter * zone.ZoneInfo.Radius * Vector3.one;
            CurrentColor = ZoneColor.Neutral;
        }

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            material = meshRenderer.material;
            colorPropertyId = Shader.PropertyToID(MaterialColor);
        }

        private void Update()
        {
            ZoneColor color = ZoneColor.Friend;

            if (!zone.State.Captured)
            {
                color = ZoneColor.Neutral;
            }
            else if (zone.State.OwnerTeam != room.Team)
            {
                color = ZoneColor.Enemy;
            }

            if (color != currentColor)
            {
                CurrentColor = color;
            }
        }

        private void OnDestroy()
        {
            if (material)
            {
                Destroy(material);
                material = null;
            }
        }
    }
}
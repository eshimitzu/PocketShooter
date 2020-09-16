using Heyworks.PocketShooter.Utils;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public class BulletImpactController
    {
        private const float RaycastDistanceToTest = 1.0f;

        private readonly HitImpactCollection impacts;

        private int playerMask;

        public BulletImpactController(HitImpactCollection impacts)
        {
            AssertUtils.NotNull(impacts, nameof(impacts));

            playerMask = LayerMask.GetMask("Player", "Enemy", "Ally", "Dead");
            this.impacts = impacts;
        }

        public void Visualize(Vector3 point, Vector3 direction)
        {
            var ray = new Ray(point - (direction.normalized * RaycastDistanceToTest / 2.0f), direction);
            ImpactInfo info;
            if (GetImpactInfo(ray, out info))
            {
                var prefab = impacts.GetImpactPrefab(info.SurfaceType);
                var impact = Lean.Pool.LeanPool.Spawn(prefab, point, Quaternion.LookRotation(info.Normal));
                impact.Play();
            }
        }

        private bool GetImpactInfo(Ray ray, out ImpactInfo info)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, RaycastDistanceToTest))
            {
                // TODO: a.dezhurko implement more robust logic
                var hitMask = 1 << hitInfo.transform.gameObject.layer;
                var surfaceType = (hitMask & playerMask) != 0 ? SurfaceType.Trooper : SurfaceType.Wall;
                info = new ImpactInfo(hitInfo.point, hitInfo.normal, surfaceType);

                return true;
            }

            info = default;

            return false;
        }

        private struct ImpactInfo
        {
            public ImpactInfo(Vector3 point, Vector3 normal, SurfaceType surfaceType)
            {
                this.Normal = normal;
                this.Point = point;
                this.SurfaceType = surfaceType;
            }

            public Vector3 Point { get; }

            public Vector3 Normal { get; }

            public SurfaceType SurfaceType { get; }
        }
    }
}

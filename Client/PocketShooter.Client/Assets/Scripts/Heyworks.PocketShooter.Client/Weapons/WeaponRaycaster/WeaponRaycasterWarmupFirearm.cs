using Heyworks.PocketShooter.Utils;
using Heyworks.PocketShooter.Weapons.AimAssistant;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public class WeaponRaycasterWarmupFirearm : WeaponRaycasterFirearm
    {
        [SerializeField]
        private LayerMask aimingLayerMask;

        public override bool Test(Ray ray, float distance, out Vector3 target)
        {
            AssertUtils.IsValidArg(distance > 0, "distance");
            AssertUtils.IsValidArg(ray.direction.magnitude > 0, "ray");

            ray = MoveRayToShotOrigin(ray);
            Debug.DrawLine(ray.origin, ray.GetPoint(distance), Color.yellow);

            var aimingMask = RaycastLayerMask | aimingLayerMask;
            var targetMask = targetLayerMask | aimingLayerMask;

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, distance, aimingMask))
            {
                target = ray.GetPoint(hitInfo.distance);

                var layerMask = 1 << hitInfo.collider.gameObject.layer;
                bool isTarget = (layerMask & targetMask) > 0;

                // check if enemy
                if (isTarget)
                {
                    var aim = hitInfo.collider.GetComponentInParent<AimAssistantTarget>();
                    isTarget = IsTarget(aim.BodyCollider.gameObject.layer);
                }

                return isTarget;
            }

            target = ray.GetPoint(distance);
            return false;
        }
    }
}
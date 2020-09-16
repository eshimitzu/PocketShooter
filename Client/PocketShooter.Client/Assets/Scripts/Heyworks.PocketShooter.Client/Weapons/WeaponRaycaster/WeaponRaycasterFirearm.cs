using Collections.Pooled;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Utils;
using Microsoft.Extensions.Logging;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public class WeaponRaycasterFirearm : WeaponRaycaster
    {
        private const float DispersionDistance = 100;

        [SerializeField]
        private string headTag = "Head";

        public bool Test(float distance, out Vector3 target)
        {
            AssertUtils.IsValidArg(distance > 0, "distance");

            var ray = MainCamera.ViewportPointToRay(AimViewportPoint);

            return Test(ray, distance, out target);
        }

        public virtual bool Test(Ray ray, float distance, out Vector3 target)
        {
            AssertUtils.IsValidArg(distance > 0, "distance");
            AssertUtils.IsValidArg(ray.direction.magnitude > 0, "ray");

            ray = MoveRayToShotOrigin(ray);
            Debug.DrawLine(ray.origin, ray.GetPoint(distance), Color.yellow);

            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, distance, RaycastLayerMask))
            {
                target = ray.GetPoint(hitInfo.distance);

                return IsTarget(hitInfo.collider.gameObject.layer);
            }

            target = ray.GetPoint(distance);
            return false;
        }

        public void Shoot(float distance, float dispersion, int fraction, float fractionDispersion, PooledList<ClientShotInfo> results)
        {
            AssertUtils.NotNull(results, "results");

            AssertUtils.IsValidArg(dispersion >= 0, "dispersion");
            AssertUtils.IsValidArg(distance > 0, "distance");
            AssertUtils.IsValidArg(fraction > 0, "fraction");

            var ray = MainCamera.ViewportPointToRay(AimViewportPoint);

            Shoot(ray, distance, dispersion, fraction, fractionDispersion, results);
        }

        public void Shoot(
            Ray ray,
            float distance,
            float dispersion,
            int fraction,
            float fractionDispersion,
            PooledList<ClientShotInfo> results)
        {
            AssertUtils.NotNull(results, "results");
            AssertUtils.IsValidArg(dispersion >= 0, "dispersion");
            AssertUtils.IsValidArg(distance > 0, "distance");
            AssertUtils.IsValidArg(fraction > 0, "fraction");

            var baseRay = GetRandomizedRay(ray, dispersion);
            baseRay = MoveRayToShotOrigin(baseRay);

            Debug.DrawLine(baseRay.origin, baseRay.GetPoint(distance), Color.red, 5);

            results.Add(CalculateShotInfo(baseRay, distance));

            for (int index = 1; index < fraction; index++)
            {
                var randomizedRay = GetRandomizedRay(baseRay, fractionDispersion);

                Debug.DrawLine(randomizedRay.origin, randomizedRay.GetPoint(distance), Color.green, 5);
                randomizedRay = MoveRayToShotOrigin(randomizedRay);
                Debug.DrawLine(randomizedRay.origin, randomizedRay.GetPoint(distance), Color.red, 5);

                results.Add(CalculateShotInfo(randomizedRay, distance));
            }
        }

        private ClientShotInfo CalculateShotInfo(Ray ray, float distance)
        {
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, distance, RaycastLayerMask))
            {
                bool hit = IsTarget(hitInfo.collider.gameObject.layer);

                var target = hit ? hitInfo.collider.gameObject.GetComponentInParent<NetworkCharacter>() : null;
                var headshot = hitInfo.collider.CompareTag(headTag);

                GameLog.Trace(
                    "Shot info calculated. Target: {Target}, point: {point}, direction: {direction}, headshot: {headshot}",
                    target,
                    hitInfo.point,
                    ray.direction,
                    headshot);

                return new ClientShotInfo(target, hitInfo.point, ray.direction, headshot);
            }

            GameLog.Trace(
                "Shot info calculated. Target: {Target}, point: {point}, direction: {direction}, headshot: {headshot}",
                "null",
                hitInfo.point,
                ray.direction,
                false);

            return new ClientShotInfo(null, ray.GetPoint(distance), ray.direction, false);
        }

        protected Ray MoveRayToShotOrigin(Ray ray)
        {
            var plane = new Plane(ray.direction, ShotOriginTransfrom.position);
            float shift;
            plane.Raycast(ray, out shift);
            ray.origin = ray.GetPoint(shift);

            return ray;
        }

        protected Ray GetRandomizedRay(Ray ray, float dispersion)
        {
            var rand = Random.insideUnitCircle * dispersion;
            var rawPoint = ray.GetPoint(DispersionDistance);
            var vectorUp = Vector3.Cross(transform.right, ray.direction);
            var vectorRight = Vector3.Cross(ray.direction, vectorUp);
            var shiftedPoint = rawPoint + (vectorRight * rand.x) + (vectorUp * rand.y);

            return new Ray(ray.origin, shiftedPoint - ray.origin);
        }
    }
}
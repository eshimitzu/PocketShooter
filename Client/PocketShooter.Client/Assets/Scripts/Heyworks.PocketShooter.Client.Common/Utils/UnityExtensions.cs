using System;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    public static class UnityExtensions
    {
        public static void RunOnChildrenRecursive(this GameObject go, Action<GameObject> action)
        {
            if (go == null) return;
            foreach (var trans in go.GetComponentsInChildren<Transform>(true))
            {
                action(trans.gameObject);
            }
        }

        /// <summary>
        /// Resets the state of the transform component (either to local or to global originals).
        /// </summary>
        /// <param name="transform">Transform to reset.</param>
        /// <param name="local">Value indicating whether local or global values should be reset.</param>
        public static void Reset(this Transform transform, bool local)
        {
            if (local)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                transform.localScale = Vector3.one;
            }
            else
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// Checks whether transform located in the inside the zone.
        /// </summary>
        /// <param name="self">Self.</param>
        /// <param name="oX">X coord of the center.</param>
        /// <param name="oZ">Y coord of the center.</param>
        /// <param name="radiusSqr">Square radius of the zone.</param>
        /// <returns>True if transform located in the inside the zone.</returns>
        public static bool IsInside(this Vector3 self, float oX, float oZ, float radiusSqr)
        {
            double dx = oX - self.x;
            double dz = oZ - self.z;
            return (dx * dx + dz * dz) < radiusSqr;
        }
    }
}
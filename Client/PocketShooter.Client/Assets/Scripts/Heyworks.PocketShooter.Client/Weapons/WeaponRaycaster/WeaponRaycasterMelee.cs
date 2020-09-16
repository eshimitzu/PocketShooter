using Collections.Pooled;
using Heyworks.PocketShooter.Networking.Actors;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public class WeaponRaycasterMelee : WeaponRaycaster
    {
        public bool MeleeHit(PooledList<ClientShotInfo> shotInfo, float hitWidth = 0.4f, float hitHeight = 0.4f, float hitDistance = 0.4f)
        {
            var boxRay = MainCamera.ViewportPointToRay(AimViewportPoint);

            RaycastHit[] boxHits = Physics.BoxCastAll(
                                                       ShotOriginTransfrom.position + boxRay.direction * hitDistance,
                                                       new Vector3(hitWidth, hitHeight, hitDistance),
                                                       boxRay.direction,
                                                       transform.rotation,
                                                       0f,
                                                       RaycastLayerMask);

            bool isEnemyInDamageZone = false;
            foreach (RaycastHit boxHit in boxHits)
            {
                isEnemyInDamageZone = IsTarget(boxHit.collider.gameObject.layer);

                if (isEnemyInDamageZone)
                {
                    RaycastHit rayHit;
                    Ray ray = new Ray(ShotOriginTransfrom.position, (boxHit.transform.position - ShotOriginTransfrom.position).normalized);
                    if (Physics.Raycast(ray, out rayHit, RaycastLayerMask))
                    {
                        bool isRayHitEnemy = IsTarget(rayHit.collider.gameObject.layer);

                        if (isRayHitEnemy)
                        {
                            Vector3 localRayHitPoint = rayHit.transform.InverseTransformPoint(rayHit.point);

                            float rayHitAngleZ = Mathf.Atan2(localRayHitPoint.z, localRayHitPoint.x) * Mathf.Rad2Deg;

                            bool isBack = rayHitAngleZ < -60f && rayHitAngleZ > -120f;

                            NetworkCharacter target = boxHit.collider.gameObject.GetComponentInParent<NetworkCharacter>();

                            shotInfo.Add(new ClientShotInfo(target, rayHit.point, boxRay.direction, isBack));
                        }
                    }

                    return isEnemyInDamageZone;
                }
            }

            return false;
        }

        /*
        void OnDrawGizmos()
        {
            var ray = MainCamera.ViewportPointToRay(AimViewportPoint);
            Gizmos.color = Color.red;

            Matrix4x4 cubeTransform = Matrix4x4.TRS(ShotOriginTransfrom.position + ray.direction * 0.4f, Quaternion.LookRotation(ray.direction), new Vector3(0.4f, 0.4f, 0.4f) * 2f);
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

            Gizmos.matrix *= cubeTransform;

            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            Gizmos.matrix = oldGizmosMatrix;
        }
        */
    }
}
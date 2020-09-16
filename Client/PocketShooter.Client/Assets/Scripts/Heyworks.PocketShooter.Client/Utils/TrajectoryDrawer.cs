using System.Collections.Generic;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils
{
    public class TrajectoryDrawer : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;
        private Collider[] colliders = new Collider[1];

        public void Draw(Vector3 startPosition, Vector3 startVelocity)
        {
            List<Vector3> positions = new List<Vector3>();

            Vector3 gravity = Physics.gravity;
            Vector3 position = startPosition;
            Vector3 velocity = startVelocity;

            for (int i = 0; i < 100; i++)
            {
                float deltaTime = 1f / 60f;
                velocity += gravity * deltaTime;
                position = position + velocity * deltaTime;
                positions.Add(position);

                if (i > 5 && Physics.OverlapSphereNonAlloc(position, 0.1f, colliders) > 0)
                {
                    break;
                }
            }

            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }
    }
}
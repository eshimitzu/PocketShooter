using UnityEngine;

namespace Heyworks.PocketShooter.Skills
{
    public class DiveRocket : MonoBehaviour
    {
        private Vector3 startPosition;
        private Transform endPoint;

        private float currentMovingTime;
        private float movingTime;

        private bool isMoving;

        public void MoveToPoint(Transform point, float time)
        {
            startPosition = transform.position;
            endPoint = point;
            currentMovingTime = 0f;
            movingTime = time;
            isMoving = true;

            transform.LookAt(endPoint);
        }

        private void Update()
        {
            if (isMoving)
            {
                if (currentMovingTime > movingTime)
                {
                    Destroy(gameObject);
                }

                currentMovingTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPosition, endPoint.position, currentMovingTime / movingTime);

                transform.LookAt(endPoint.position);
            }
        }
    }
}
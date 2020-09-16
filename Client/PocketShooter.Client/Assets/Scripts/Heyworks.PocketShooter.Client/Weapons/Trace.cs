using System.Collections;
using Lean.Pool;
using UnityEngine;

namespace Heyworks.PocketShooter.Weapons
{
    public class Trace : MonoBehaviour
    {
        [SerializeField]
        private float traceSpeed = 10f;

        private Coroutine coroutine;

        public void Move(Vector3 startPoint, Vector3 endPoint)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            iTween.Stop(gameObject);

            transform.position = startPoint;
            coroutine = StartCoroutine(MoveCoroutine(startPoint, endPoint));
        }

        private IEnumerator MoveCoroutine(Vector3 startPoint, Vector3 endPoint)
        {
            yield return null;

            var progress = 0f;

            while (progress <= 1.0f)
            {
                progress += Time.deltaTime * traceSpeed;
                transform.position = Vector3.Lerp(startPoint, endPoint, progress);

                yield return null;
            }

            LeanPool.Despawn(gameObject);
        }
    }
}
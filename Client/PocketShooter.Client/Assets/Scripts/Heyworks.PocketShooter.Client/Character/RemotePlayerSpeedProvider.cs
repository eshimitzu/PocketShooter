using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    /// <summary>
    /// Calculates speed of the remote character.
    /// TODO: send over network instead of calculating (?).
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    /// <seealso cref="IAnimationSpeedProvider" />
    public class RemotePlayerSpeedProvider : MonoBehaviour, IAnimationSpeedProvider
    {
        [SerializeField]
        private CharacterControllerSettings settings;
        [SerializeField]
        private int bufferLength = 10;

        private Vector3[] buffer;

        /// <summary>
        /// Gets the current speed along forward axis.
        /// </summary>
        public float CurrentForwardSpeed
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current speed along right axis.
        /// </summary>
        public float CurrentRightSpeed
        {
            get;
            private set;
        }

        private void Start()
        {
            buffer = new Vector3[bufferLength];

            for (int i = 0; i < bufferLength; i++)
            {
                buffer[i] = transform.position;
            }
        }

        private void Update()
        {
            var move = transform.position - buffer[0];

            if (move.magnitude <= Mathf.Epsilon)
            {
                CurrentForwardSpeed = 0;
                CurrentRightSpeed = 0;
            }
            else
            {
                var scale = settings.GetSpeedScaleCoefficient(move);

                CurrentForwardSpeed = Vector3.Dot(transform.forward, move) / (scale * settings.MaxStableMoveSpeed * Time.deltaTime * bufferLength);
                CurrentRightSpeed = Vector3.Dot(transform.right, move) / (scale * settings.MaxStableMoveSpeed * Time.deltaTime * bufferLength);
            }

            AddPositionToBuffer(transform.position);
        }

        private void AddPositionToBuffer(Vector3 position)
        {
            for (int i = 0; i < bufferLength - 1; i++)
            {
                buffer[i] = buffer[i + 1];
            }

            buffer[bufferLength - 1] = position;
        }
    }
}

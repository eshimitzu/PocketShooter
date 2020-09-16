using Heyworks.PocketShooter.Networking.Actors;
using UnityEngine;

namespace Heyworks.PocketShooter.Client
{
    public abstract class EffectController : MonoBehaviour
    {
        protected NetworkCharacter Character { get; private set; }

        public virtual void Setup(NetworkCharacter character)
        {
            Character = character;
        }

        public abstract void Stop();
    }
}
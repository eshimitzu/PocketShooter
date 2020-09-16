using Heyworks.PocketShooter.Networking.Actors;
using UnityEngine;

namespace Heyworks.PocketShooter.Modules.GameEnvironment
{
    /// <summary>
    /// Represents the border behaviour of the map.
    /// Kill the local player whether he falls down.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class MapBorder : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            LocalCharacter localCharacter = other.gameObject.GetComponent<LocalCharacter>();
            if (localCharacter != null)
            {
                // TODO: v.filippov kill outside map
                // localPlayer.Kill();
            }
        }
    }
}
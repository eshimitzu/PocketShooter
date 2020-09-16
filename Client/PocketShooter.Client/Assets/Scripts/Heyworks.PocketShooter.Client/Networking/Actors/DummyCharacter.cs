using Heyworks.PocketShooter.Character;
using UnityEngine;
using Zenject;

namespace Heyworks.PocketShooter.Networking.Actors
{
    public class DummyCharacter : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<UnityEngine.Object, DummyCharacter>
        {
        }

        [SerializeField]
        private CharacterCommon characterCommon;
        [SerializeField]
        private RemotePlayerSpeedProvider remotePlayerSpeedProvider;

        public CharacterCommon CharacterCommon => characterCommon;

        public RemotePlayerSpeedProvider RemotePlayerSpeedProvider => remotePlayerSpeedProvider;
    }
}
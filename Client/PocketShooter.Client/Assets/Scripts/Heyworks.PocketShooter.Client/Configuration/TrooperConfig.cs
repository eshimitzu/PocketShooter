using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Networking.Actors;
using UnityEngine;

namespace Heyworks.PocketShooter.Configuration
{
    /// <summary>
    /// Represents trooper configuration.
    /// </summary>
    [System.Serializable]
    public class TrooperConfig
    {
        [SerializeField]
        private TrooperClass trooperClass = TrooperClass.Rambo;
        [SerializeField]
        private TrooperAvatar trooperAvatar;
        [SerializeField]
        private BotCharacter botCharacter;

        public TrooperClass TrooperClass => trooperClass;

        public BotCharacter BotCharacter => botCharacter;

        public TrooperAvatar TrooperAvatar => trooperAvatar;

        public TrooperConfig()
        {
        }

        public TrooperConfig(TrooperClass trooperClass, TrooperAvatar trooperAvatar, BotCharacter botCharacter)
        {
            this.trooperClass = trooperClass;
            this.trooperAvatar = trooperAvatar;
            this.botCharacter = botCharacter;
        }
    }
}
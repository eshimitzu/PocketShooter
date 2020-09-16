using System.Collections.Generic;
using Heyworks.PocketShooter.AnimationUtility;
using Heyworks.PocketShooter.Character;
using Heyworks.PocketShooter.Networking.Actors;
using Heyworks.PocketShooter.Realtime.Data;
using Heyworks.PocketShooter.Utils;
using UnityEngine;

namespace Heyworks.PocketShooter.Configuration
{
    /// <summary>
    /// Represents trooper configuration.
    /// </summary>
    [CreateAssetMenu(fileName = "TroopersConfig", menuName = "HeyworksMain/Settings/Troopers Configuration")]
    public class TroopersConfig : ScriptableObject
    {
        [SerializeField]
        private List<TrooperConfig> troopers;
        [SerializeField]
        private RemoteCharacter remoteCharacter;
        [SerializeField]
        private LocalCharacter localCharacter;
        [SerializeField]
        private TrooperMaterialConfig trooperMaterialConfig;
        [SerializeField]
        private float deadHitPower = 100f;

        public RemoteCharacter RemoteCharacter => remoteCharacter;

        public LocalCharacter LocalCharacter => localCharacter;

        public TrooperMaterialConfig TrooperMaterialConfig => trooperMaterialConfig;

        public float DeadHitPower => deadHitPower;

        public IEnumerable<TrooperConfig> Troopers => troopers;

        public TrooperConfig GetTrooperWithClass(TrooperClass trooperClass)
        {
            TrooperConfig trooperConfig = troopers.Find(x => x.TrooperClass == trooperClass);

            AssertUtils.NotNull(trooperConfig, "Trooper config doesn't have trooper with class " + trooperClass);

            return trooperConfig;
        }

        public void AddTrooperToTroopersConfig(TrooperConfig trooperConfig)
        {
            troopers.Add(trooperConfig);
        }
    }
}
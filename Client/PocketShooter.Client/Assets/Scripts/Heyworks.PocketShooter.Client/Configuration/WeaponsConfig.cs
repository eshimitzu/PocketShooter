using System.Collections.Generic;
using Heyworks.PocketShooter.Utils;
using UnityEngine;

namespace Heyworks.PocketShooter.Configuration
{
    [CreateAssetMenu(fileName = "WeaponsConfig", menuName = "HeyworksMain/Settings/Create Weapons Configuration")]
    public class WeaponsConfig : ScriptableObject
    {
        [SerializeField]
        private List<WeaponViewConfig> weaponList;

        public IEnumerable<WeaponViewConfig> WeaponList => weaponList;

        public WeaponViewConfig GetWeaponByName(WeaponName weaponName)
        {
            WeaponViewConfig weaponConfig = weaponList.Find(x => x.Name == weaponName);

            AssertUtils.NotNull(weaponConfig, "Weapon config doesn't have weapon with name " + weaponName);

            return weaponConfig;
        }
    }
}
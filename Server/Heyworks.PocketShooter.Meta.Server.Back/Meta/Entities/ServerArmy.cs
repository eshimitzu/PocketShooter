using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Runnables;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ServerArmy : ArmyBase
    {
        private readonly ServerArmyState armyState;
        private readonly IArmyConfigurationBase armyConfiguration;
        private readonly ITrooperConfiguration trooperConfiguration;
        private readonly IWeaponConfiguration weaponConfiguration;
        private readonly IHelmetConfiguration helmetConfiguration;
        private readonly IArmorConfiguration armorConfiguration;
        private readonly ISkillConfiguration skillConfiguration;

        public ServerArmy(
            ServerArmyState armyState,
            IDateTimeProvider dateTimeProvider,
            IArmyConfigurationBase armyConfiguration,
            ITrooperConfiguration trooperConfiguration,
            IWeaponConfiguration weaponConfiguration,
            IHelmetConfiguration helmetConfiguration,
            IArmorConfiguration armorConfiguration,
            ISkillConfiguration skillConfiguration)
            : base(armyState, dateTimeProvider)
        {
            this.armyState = armyState;
            this.armyConfiguration = armyConfiguration;
            this.trooperConfiguration = trooperConfiguration;
            this.weaponConfiguration = weaponConfiguration;
            this.helmetConfiguration = helmetConfiguration;
            this.armorConfiguration = armorConfiguration;
            this.skillConfiguration = skillConfiguration;
        }

        public IReadOnlyList<ServerTrooper> Troopers =>
            armyState.Troopers
            .Select(item => new ServerTrooper(
                armyState,
                item,
                trooperConfiguration,
                weaponConfiguration,
                helmetConfiguration,
                armorConfiguration,
                skillConfiguration))
            .ToList();

        public IReadOnlyList<ServerWeapon> Weapons =>
            armyState.Weapons.Select(item => new ServerWeapon(item, weaponConfiguration)).ToList();

        public IReadOnlyList<ServerHelmet> Helmets =>
            armyState.Helmets.Select(item => new ServerHelmet(item, helmetConfiguration)).ToList();

        public IReadOnlyList<ServerArmor> Armors =>
            armyState.Armors.Select(item => new ServerArmor(item, armorConfiguration)).ToList();

        public ServerArmyState GetState()
        {
            // TODO: Think about state cloning to prevent modifications from outside.
            return armyState;
        }

        public ServerTrooper GetTrooper(TrooperClass trooperClass) =>
            (ServerTrooper)FindTrooper(trooperClass) ?? throw new InvalidOperationException($"Can't find trooper with class {trooperClass}");

        public ServerWeapon GetWeapon(WeaponName weaponName) =>
            (ServerWeapon)FindWeapon(weaponName) ?? throw new InvalidOperationException($"Can't find weapon with name {weaponName}");

        public ServerHelmet GetHelmet(HelmetName helmetName) =>
            (ServerHelmet)FindHelmet(helmetName) ?? throw new InvalidOperationException($"Can't find helmet with name {helmetName}");

        public ArmyItemProgressBase GetItemProgress()
        {
            return
                HasItemProgress
                ? new ArmyItemProgressBase(this, armyState.ItemProgress, DateTimeProvider, armyConfiguration)
                : null;
        }

        public ServerArmor GetArmor(ArmorName armorName) =>
            (ServerArmor)FindArmor(armorName) ?? throw new InvalidOperationException($"Can't find armor with name {armorName}");

        public override ArmyItem FindItem(int id)
        {
            var trooperState = armyState.Troopers.FirstOrDefault(_ => _.Id == id);
            if (trooperState != null)
            {
                return new ServerTrooper(
                    armyState,
                    trooperState,
                    trooperConfiguration,
                    weaponConfiguration,
                    helmetConfiguration,
                    armorConfiguration,
                    skillConfiguration);
            }

            var weaponState = armyState.Weapons.FirstOrDefault(_ => _.Id == id);
            if (weaponState != null)
            {
                return new ServerWeapon(weaponState, weaponConfiguration);
            }

            var armorState = armyState.Armors.FirstOrDefault(_ => _.Id == id);
            if (armorState != null)
            {
                return new ServerArmor(armorState, armorConfiguration);
            }

            var helmetState = armyState.Helmets.FirstOrDefault(_ => _.Id == id);
            if (helmetState != null)
            {
                return new ServerHelmet(helmetState, helmetConfiguration);
            }

            return null;
        }

        protected override TrooperBase FindTrooper(TrooperClass trooperClass) =>
            Troopers.FirstOrDefault(_ => _.Class == trooperClass);

        protected override WeaponBase FindWeapon(WeaponName weaponName) =>
            Weapons.FirstOrDefault(_ => _.Name == weaponName);

        protected override HelmetBase FindHelmet(HelmetName helmetName) =>
            Helmets.FirstOrDefault(_ => _.Name == helmetName);

        protected override ArmorBase FindArmor(ArmorName armorName) =>
            Armors.FirstOrDefault(_ => _.Name == armorName);

        protected override IEnumerable<IRunnable> GetRunnables()
        {
            if (HasItemProgress)
            {
                yield return GetItemProgress();
            }
        }
    }
}

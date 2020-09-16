using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Runnables;
using Heyworks.PocketShooter.Meta.Communication;
using UniRx;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class Army : ArmyBase
    {
        private ArmyState armyState;
        private readonly IGameHubClient gameHubClient;
        private readonly IArmyConfigurationBase armyConfiguration;
        private readonly ITrooperConfigurationBase trooperConfiguration;
        private readonly IWeaponConfigurationBase weaponConfiguration;
        private readonly IHelmetConfigurationBase helmetConfiguration;
        private readonly IArmorConfigurationBase armorConfiguration;

        public ISubject<ArmyItemProgress> ItemProgressCompleted = new Subject<ArmyItemProgress>();
        public ISubject<ArmyItemProgress> ItemProgressStarted = new Subject<ArmyItemProgress>();

        public Army(
            ArmyState armyState,
            IDateTimeProvider dateTimeProvider,
            IGameHubClient gameHubClient,
            IArmyConfigurationBase armyConfiguration,
            ITrooperConfigurationBase trooperConfiguration,
            IWeaponConfigurationBase weaponConfiguration,
            IHelmetConfigurationBase helmetConfiguration,
            IArmorConfigurationBase armorConfiguration)
            : base(armyState, dateTimeProvider)
        {
            this.armyState = armyState;
            this.gameHubClient = gameHubClient;
            this.armyConfiguration = armyConfiguration;
            this.trooperConfiguration = trooperConfiguration;
            this.weaponConfiguration = weaponConfiguration;
            this.helmetConfiguration = helmetConfiguration;
            this.armorConfiguration = armorConfiguration;
        }

        public override void UpdateState(ArmyState state)
        {
            base.UpdateState(state);
            armyState = state;
        }

        public IReadOnlyList<Trooper> Troopers =>
            armyState.Troopers
            .Select(item => new Trooper(
                armyState,
                item,
                trooperConfiguration,
                weaponConfiguration,
                helmetConfiguration,
                armorConfiguration))
            .ToList();

        public IReadOnlyList<Weapon> Weapons =>
            armyState.Weapons.Select(item => new Weapon(item, weaponConfiguration)).ToList();

        public IReadOnlyList<Armor> Armors =>
            armyState.Armors.Select(item => new Armor(item, armorConfiguration)).ToList();

        public IReadOnlyList<Helmet> Helmets =>
            armyState.Helmets.Select(item => new Helmet(item, helmetConfiguration)).ToList();

        public IContentIdentity FindContent(int id)
        {
            var trooperState = armyState.Troopers.FirstOrDefault(_ => _.Id == id);
            if (trooperState != null)
            {
                return new TrooperIdentity(trooperState.Class, trooperState.Grade, trooperState.Level);
            }

            var weaponState = armyState.Weapons.FirstOrDefault(_ => _.Id == id);
            if (weaponState != null)
            {
                return new WeaponIdentity(weaponState.Name, weaponState.Grade, weaponState.Level);
            }

            var armorState = armyState.Armors.FirstOrDefault(_ => _.Id == id);
            if (armorState != null)
            {
                return new ArmorIdentity(armorState.Name, armorState.Grade, armorState.Level);
            }

            var helmetState = armyState.Helmets.FirstOrDefault(_ => _.Id == id);
            if (helmetState != null)
            {
                return new HelmetIdentity(helmetState.Name, helmetState.Grade, helmetState.Level);
            }

            return null;
        }

        public override ArmyItem FindItem(int id)
        {
            var trooperState = armyState.Troopers.FirstOrDefault(_ => _.Id == id);
            if (trooperState != null)
            {
                return new Trooper(
                    armyState,
                    trooperState,
                    trooperConfiguration,
                    weaponConfiguration,
                    helmetConfiguration,
                    armorConfiguration);
            }

            var weaponState = armyState.Weapons.FirstOrDefault(_ => _.Id == id);
            if (weaponState != null)
            {
                return new Weapon(weaponState, weaponConfiguration);
            }

            var armorState = armyState.Armors.FirstOrDefault(_ => _.Id == id);
            if (armorState != null)
            {
                return new Armor(armorState, armorConfiguration);
            }

            var helmetState = armyState.Helmets.FirstOrDefault(_ => _.Id == id);
            if (helmetState != null)
            {
                return new Helmet(helmetState, helmetConfiguration);
            }

            return null;
        }

        public ArmyItemProgress GetItemProgress()
        {
            if (HasItemProgress)
            {
                return new ArmyItemProgress(
                    this,
                    armyState.ItemProgress,
                    DateTimeProvider,
                    armyConfiguration,
                    gameHubClient);
            }

            return null;
        }

        protected override void OnItemProgressStarted()
        {
            base.OnItemProgressStarted();

            ItemProgressStarted.OnNext(GetItemProgress());
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
            var progress = GetItemProgress();
            if (progress != null)
            {
                yield return progress;
            }
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Communication;
using NUnit.Framework;

namespace Heyworks.PocketShooter
{
    public class ArmyGeneratorTests
    {
        [Test]
        public void GeneratAny()
        {
            var generator = new ArmyGenerator();
            var defaults = new List<(TrooperClass, Grade)>
            {
                (TrooperClass.Rambo, Grade.Common),
                (TrooperClass.Statham, Grade.Rare),
                (TrooperClass.Rock, Grade.Legendary),
            };

            var weapons = new List<(WeaponName, Grade)>
            {
                (WeaponName.Barret, Grade.Common),
                (WeaponName.Bazooka, Grade.Rare),
                (WeaponName.Knife, Grade.Legendary),
            };

            var helmets = new List<(HelmetName, Grade)>
            {
                (HelmetName.Helmet1, Grade.Common),
                (HelmetName.Helmet2, Grade.Rare)
            };

            var armors = new List<(ArmorName, Grade)>
            {
                (ArmorName.Armor1, Grade.Common),
                (ArmorName.Armor2, Grade.Rare),
            };
            var trainConfig = new BotsTrainConfig { MaximalGrade = Grade.Common };
            var army = generator.Generate(Guid.NewGuid(), 1, trainConfig , new GradesDefaultsData { Troopers = defaults, Armors = armors, Helmets = helmets, Weapons = weapons });
            Assert.AreEqual(1, army.Troopers.Count);
            Assert.True(army.Troopers.Any(x => x.Class == TrooperClass.Rambo));
            Assert.True(army.Armors.All(x => x.Grade == Grade.Common));
            Assert.True(army.Helmets.All(x => x.Grade == Grade.Common));
            Assert.True(army.Weapons.All(x => x.Grade == Grade.Common));
            Assert.True(army.Weapons.Where(x => x.Name == WeaponName.Barret).Count() == 1);
        }

        [Test]
        public void GeneratMid()
        {
            var generator = new ArmyGenerator();
            var defaults = new List<(TrooperClass, Grade)>
            {
                (TrooperClass.Rambo, Grade.Common),
                (TrooperClass.Statham, Grade.Rare),
                (TrooperClass.Rock, Grade.Legendary),
            };

            var weapons = new List<(WeaponName, Grade)>
            {
                (WeaponName.Barret, Grade.Common),
                (WeaponName.Bazooka, Grade.Rare),
                (WeaponName.Knife, Grade.Legendary),
            };

            var helmets = new List<(HelmetName, Grade)>
            {
                (HelmetName.Helmet1, Grade.Common),
                (HelmetName.Helmet2, Grade.Rare)
            };

            var armors = new List<(ArmorName, Grade)>
            {
                (ArmorName.Armor1, Grade.Common),
                (ArmorName.Armor2, Grade.Rare),
            };

            var army = generator.Generate(Guid.NewGuid(), 3, new BotsTrainConfig(), new GradesDefaultsData { Troopers = defaults, Armors = armors, Helmets = helmets, Weapons = weapons });
            Assert.AreEqual(3, army.Troopers.Count);
            
            Assert.True(army.Troopers.Any(x => x.Class == TrooperClass.Rambo));
            Assert.True(army.Troopers.Any(x => x.Class == TrooperClass.Statham));
            Assert.True(army.Weapons.Where(x => x.Name == WeaponName.Bazooka).Count() == 1);
            Assert.True(army.Weapons.Where(x => x.Name == WeaponName.Knife).Count() == 1);
        }

        [Test]
        public void GeneratLarge()
        {
            var generator = new ArmyGenerator();
            var defaults = new List<(TrooperClass, Grade)>
            {
                (TrooperClass.Rambo, Grade.Common),
                (TrooperClass.Statham, Grade.Rare),
                (TrooperClass.Rock, Grade.Legendary),
            };

            var weapons = new List<(WeaponName, Grade)>
            {
                (WeaponName.Barret, Grade.Common),
                (WeaponName.Bazooka, Grade.Rare),
                (WeaponName.Knife, Grade.Legendary),
            };

            var helmets = new List<(HelmetName, Grade)>
            {
                (HelmetName.Helmet1, Grade.Common),
                (HelmetName.Helmet2, Grade.Rare)
            };

            var armors = new List<(ArmorName, Grade)>
            {
                (ArmorName.Armor1, Grade.Common),
                (ArmorName.Armor2, Grade.Rare),
            };

            var army = generator.Generate(Guid.NewGuid(), 10, new BotsTrainConfig(), new GradesDefaultsData { Troopers = defaults, Armors = armors, Helmets = helmets, Weapons = weapons });
            Assert.AreEqual(3, army.Troopers.Count);
            Assert.True(army.Troopers.Any(x => x.Class == TrooperClass.Rambo));
            Assert.True(army.Troopers.Any(x => x.Class == TrooperClass.Statham));
        }
    }
}
